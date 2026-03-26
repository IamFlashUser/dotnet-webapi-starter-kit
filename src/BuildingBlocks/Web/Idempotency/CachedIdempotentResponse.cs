namespace FSH.Framework.Web.Idempotency;

/// <summary>
/// Represents a cached HTTP response for idempotent replay.
/// </summary>
public sealed class CachedIdempotentResponse
{
    public int StatusCode { get; init; }
    public string? ContentType { get; init; }
    public byte[] Body { get; init; } = [];
}
