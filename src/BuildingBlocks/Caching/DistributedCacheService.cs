using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace FSH.Framework.Caching;

/// <summary>
/// Implementation of <see cref="ICacheService"/> using distributed cache (Redis or in-memory).
/// Provides JSON serialization for cached objects with configurable expiration policies.
/// </summary>
public sealed partial class DistributedCacheService : ICacheService
{
    private static readonly Encoding Utf8 = Encoding.UTF8;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly IDistributedCache _cache;
    private readonly ILogger<DistributedCacheService> _logger;
    private readonly CachingOptions _opts;

    /// <summary>
    /// Initializes a new instance of <see cref="DistributedCacheService"/>.
    /// </summary>
    /// <param name="cache">The underlying distributed cache implementation.</param>
    /// <param name="logger">Logger for cache operations.</param>
    /// <param name="opts">Caching configuration options.</param>
    public DistributedCacheService(
        IDistributedCache cache,
        ILogger<DistributedCacheService> logger,
        IOptions<CachingOptions> opts)
    {
        ArgumentNullException.ThrowIfNull(opts);

        _cache = cache;
        _logger = logger;
        _opts = opts.Value;
    }

    /// <inheritdoc />
    public async Task<T?> GetItemAsync<T>(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            var bytes = await _cache.GetAsync(key, ct).ConfigureAwait(false);
            if (bytes is null || bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(Utf8.GetString(bytes), JsonOpts);
        }
        // Graceful degradation: cache failures must not crash the caller — return default and log
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache get failed for key (length={KeyLength})", key.Length);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetItemAsync<T>(string key, T value, TimeSpan? sliding = default, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            var bytes = Utf8.GetBytes(JsonSerializer.Serialize(value, JsonOpts));
            await _cache.SetAsync(key, bytes, BuildEntryOptions(sliding), ct).ConfigureAwait(false);
            LogCached(key);
        }
        // Graceful degradation: cache failures must not crash the caller
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Cache set failed for key (length={KeyLength})", key.Length);
        }
    }

    /// <inheritdoc />
    public async Task SetItemAsync<T>(string key, T value, IReadOnlyList<string> tags, TimeSpan? sliding = default, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(tags);
        await SetItemAsync(key, value, sliding, ct).ConfigureAwait(false);

        // Store the key in each tag's set so we can invalidate by tag later
        foreach (var tag in tags)
        {
            var tagKey = NormalizeTagKey(tag);
            var existing = await GetItemAsync<HashSet<string>>(tagKey, ct).ConfigureAwait(false) ?? [];
            existing.Add(Normalize(key));
            await SetItemAsync(tagKey, existing, sliding, ct).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task RemoveItemAsync(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        // Graceful degradation: cache failures must not crash the caller
        try { await _cache.RemoveAsync(key, ct).ConfigureAwait(false); }
        catch (Exception ex) when (ex is not OperationCanceledException)
        { _logger.LogWarning(ex, "Cache remove failed for {Key}", key); }
    }

    /// <inheritdoc />
    public async Task RefreshItemAsync(string key, CancellationToken ct = default)
    {
        key = Normalize(key);
        try
        {
            await _cache.RefreshAsync(key, ct).ConfigureAwait(false);
            LogRefreshed(key);
        }
        // Graceful degradation: cache operations must not crash the caller
        catch (Exception ex) when (ex is not OperationCanceledException)
        { _logger.LogWarning(ex, "Cache refresh failed for {Key}", key); }
    }

    /// <inheritdoc />
    public async Task RemoveByTagAsync(string tag, CancellationToken ct = default)
    {
        var tagKey = NormalizeTagKey(tag);
        var keys = await GetItemAsync<HashSet<string>>(tagKey, ct).ConfigureAwait(false);
        if (keys is null || keys.Count == 0) return;

        foreach (var key in keys)
        {
            try
            {
                await _cache.RemoveAsync(key, ct).ConfigureAwait(false);
            }
            // Graceful degradation: best-effort removal per key
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "Cache remove by tag failed for {Key}", key);
            }
        }

        await RemoveItemAsync(tagKey, ct).ConfigureAwait(false);
    }

    private string NormalizeTagKey(string tag) => Normalize($"__tag:{tag}");

    /// <summary>
    /// Builds cache entry options with configured expiration settings.
    /// </summary>
    /// <param name="sliding">Optional sliding expiration override.</param>
    /// <returns>Configured cache entry options.</returns>
    private DistributedCacheEntryOptions BuildEntryOptions(TimeSpan? sliding)
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

    [LoggerMessage(Level = LogLevel.Debug, Message = "Cached {Key}")]
    private partial void LogCached(string key);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Refreshed {Key}")]
    private partial void LogRefreshed(string key);
}