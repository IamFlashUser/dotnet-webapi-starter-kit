using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FSH.Framework.Caching;

/// <summary>
/// A hybrid cache implementation combining L1 (in-memory) and L2 (distributed) caching.
/// Provides fast local access with distributed cache backup for multi-instance scenarios.
/// </summary>
/// <remarks>
/// The hybrid approach uses memory cache for fast L1 access and automatically populates
/// it from the L2 distributed cache on cache misses. Write operations update both caches.
/// Memory cache uses 80% of the distributed cache sliding expiration for faster refresh.
/// </remarks>
public sealed partial class HybridCacheService : ICacheService
{
    private static readonly Encoding Utf8 = Encoding.UTF8;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;
    private readonly ILogger<HybridCacheService> _logger;
    private readonly CachingOptions _opts;

    /// <summary>
    /// Initializes a new instance of <see cref="HybridCacheService"/>.
    /// </summary>
    /// <param name="memoryCache">The L1 in-memory cache.</param>
    /// <param name="distributedCache">The L2 distributed cache (Redis or memory-based).</param>
    /// <param name="logger">Logger for cache operations.</param>
    /// <param name="opts">Caching configuration options.</param>
    public HybridCacheService(
        IMemoryCache memoryCache,
        IDistributedCache distributedCache,
        ILogger<HybridCacheService> logger,
        IOptions<CachingOptions> opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
        _logger = logger;
        _opts = opts.Value;
    }

    /// <inheritdoc />
    /// <remarks>
    /// First checks L1 memory cache, then falls back to L2 distributed cache.
    /// If found in L2, the item is automatically populated into L1 for subsequent fast access.
    /// </remarks>
    public async Task<T?> GetItemAsync<T>(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            // Check L1 cache first (memory)
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                LogMemoryHit(key);
                return memoryValue;
            }

            // Fall back to L2 cache (distributed)
            var bytes = await _distributedCache.GetAsync(key, ct).ConfigureAwait(false);
            if (bytes is null || bytes.Length == 0) return default;

            var value = JsonSerializer.Deserialize<T>(Utf8.GetString(bytes), JsonOpts);

            // Populate L1 cache from L2
            if (value is not null)
            {
                var expiration = GetMemoryCacheExpiration();
                _memoryCache.Set(key, value, expiration);
                LogPopulatedFromDistributed(key);
            }

            return value;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache get failed for key (length={KeyLength})", key.Length);
            return default;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Writes to both L1 memory cache and L2 distributed cache simultaneously.
    /// </remarks>
    public async Task SetItemAsync<T>(string key, T value, TimeSpan? sliding = default, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            var bytes = Utf8.GetBytes(JsonSerializer.Serialize(value, JsonOpts));
            await _distributedCache.SetAsync(key, bytes, BuildDistributedEntryOptions(sliding), ct).ConfigureAwait(false);

            // Also set in memory cache
            var expiration = GetMemoryCacheExpiration();
            _memoryCache.Set(key, value, expiration);

            LogCachedBoth(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache set failed for key (length={KeyLength})", key.Length);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Removes from both L1 memory cache and L2 distributed cache.
    /// </remarks>
    public async Task RemoveItemAsync(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            // Remove from both caches
            _memoryCache.Remove(key);
            await _distributedCache.RemoveAsync(key, ct).ConfigureAwait(false);
            LogRemovedBoth(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache remove failed for {Key}", key);
        }
    }

    /// <inheritdoc />
    public async Task RefreshItemAsync(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            await _distributedCache.RefreshAsync(key, ct).ConfigureAwait(false);
            LogRefreshed(key);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache refresh failed for {Key}", key);
        }
    }

    /// <summary>
    /// Builds distributed cache entry options with configured expiration settings.
    /// </summary>
    /// <param name="sliding">Optional sliding expiration override.</param>
    /// <returns>Configured distributed cache entry options.</returns>
    private DistributedCacheEntryOptions BuildDistributedEntryOptions(TimeSpan? sliding)
    {
        var o = new DistributedCacheEntryOptions();

        if (sliding.HasValue)
            o.SetSlidingExpiration(sliding.Value);
        else if (_opts.DefaultSlidingExpiration.HasValue)
            o.SetSlidingExpiration(_opts.DefaultSlidingExpiration.Value);

        if (_opts.DefaultAbsoluteExpiration.HasValue)
            o.SetAbsoluteExpiration(_opts.DefaultAbsoluteExpiration.Value);

        return o;
    }

    /// <summary>
    /// Gets memory cache expiration options, set to 80% of distributed cache expiration
    /// for faster refresh cycles.
    /// </summary>
    /// <returns>Memory cache entry options with sliding expiration.</returns>
    private MemoryCacheEntryOptions GetMemoryCacheExpiration()
    {
        var options = new MemoryCacheEntryOptions();

        // Use shorter expiration for memory cache (faster refresh from distributed cache)
        var slidingExpiration = _opts.DefaultSlidingExpiration ?? TimeSpan.FromMinutes(1);
        options.SetSlidingExpiration(TimeSpan.FromSeconds(slidingExpiration.TotalSeconds * 0.8)); // 80% of distributed cache expiration

        return options;
    }

    /// <summary>
    /// Normalizes the cache key by applying the configured prefix.
    /// </summary>
    /// <param name="key">The original cache key.</param>
    /// <returns>The normalized key with prefix applied.</returns>
    /// <exception cref="ArgumentNullException">Thrown when key is null or whitespace.</exception>
    private string Normalize(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        var prefix = _opts.KeyPrefix ?? string.Empty;
        if (prefix.Length == 0)
        {
            return key;
        }

        return key.StartsWith(prefix, StringComparison.Ordinal)
            ? key
            : prefix + key;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cache hit in memory for {Key}")]
    private partial void LogMemoryHit(string key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Populated memory cache from distributed cache for {Key}")]
    private partial void LogPopulatedFromDistributed(string key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cached {Key} in both memory and distributed caches")]
    private partial void LogCachedBoth(string key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Removed {Key} from both caches")]
    private partial void LogRemovedBoth(string key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Refreshed {Key}")]
    private partial void LogRefreshed(string key);
}