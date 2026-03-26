using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Web.Sse;

/// <summary>
/// Manages active SSE connections per user. Supports targeted sends (by userId)
/// and tenant-wide broadcasts. Thread-safe via ConcurrentDictionary.
/// </summary>
public sealed class SseConnectionManager
{
    private readonly ConcurrentDictionary<string, Channel<SseEvent>> _connections = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, string> _userTenantMap = new(StringComparer.Ordinal);
    private readonly ILogger<SseConnectionManager> _logger;

    public SseConnectionManager(ILogger<SseConnectionManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a channel for the user and returns a reader to consume events.
    /// </summary>
    public ChannelReader<SseEvent> Connect(string userId, string? tenantId = null)
    {
        var channel = Channel.CreateBounded<SseEvent>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

        _connections[userId] = channel;
        if (tenantId is not null)
        {
            _userTenantMap[userId] = tenantId;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("SSE client connected: {UserId} (tenant: {TenantId})", userId, tenantId ?? "none");
        }

        return channel.Reader;
    }

    /// <summary>
    /// Disconnects a user and completes their channel.
    /// </summary>
    public void Disconnect(string userId)
    {
        if (_connections.TryRemove(userId, out var channel))
        {
            channel.Writer.TryComplete();
            _userTenantMap.TryRemove(userId, out _);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("SSE client disconnected: {UserId}", userId);
            }
        }
    }

    /// <summary>
    /// Sends an event to a specific user. Returns false if the user is not connected.
    /// </summary>
    public bool TrySend(string userId, SseEvent sseEvent)
    {
        if (_connections.TryGetValue(userId, out var channel))
        {
            return channel.Writer.TryWrite(sseEvent);
        }

        return false;
    }

    /// <summary>
    /// Broadcasts an event to all connected users in a specific tenant.
    /// </summary>
    public int Broadcast(string tenantId, SseEvent sseEvent)
    {
        var sent = 0;
        foreach (var (userId, tid) in _userTenantMap)
        {
            if (string.Equals(tid, tenantId, StringComparison.Ordinal) && TrySend(userId, sseEvent))
            {
                sent++;
            }
        }

        return sent;
    }

    /// <summary>
    /// Broadcasts an event to ALL connected users (cross-tenant).
    /// </summary>
    public int BroadcastAll(SseEvent sseEvent)
    {
        var sent = 0;
        foreach (var (_, channel) in _connections)
        {
            if (channel.Writer.TryWrite(sseEvent))
            {
                sent++;
            }
        }

        return sent;
    }

    /// <summary>
    /// Gets the number of active connections.
    /// </summary>
    public int ActiveConnections => _connections.Count;
}
