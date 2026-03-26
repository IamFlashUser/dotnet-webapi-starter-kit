using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Web.Sse;

public static class SseEndpoints
{
    /// <summary>
    /// Maps the SSE streaming endpoint at /api/v1/sse/stream.
    /// Requires authentication. Streams events as text/event-stream.
    /// </summary>
    public static IEndpointRouteBuilder MapHeroSseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapGet("/api/v1/sse/stream", async (
            HttpContext context,
            SseConnectionManager connectionManager,
            CancellationToken cancellationToken) =>
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var tenantId = context.User.FindFirst("tenant")?.Value;

            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";
            context.Response.Headers["X-Accel-Buffering"] = "no"; // Disable proxy buffering (nginx)

            var reader = connectionManager.Connect(userId, tenantId);

            try
            {
                await foreach (var sseEvent in reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                {
                    if (sseEvent.Id is not null)
                    {
                        await context.Response.WriteAsync($"id: {sseEvent.Id}\n", cancellationToken).ConfigureAwait(false);
                    }

                    await context.Response.WriteAsync($"event: {sseEvent.EventType}\n", cancellationToken).ConfigureAwait(false);

                    // SSE spec: multi-line data needs each line prefixed with "data: "
                    foreach (var line in sseEvent.Data.Split('\n'))
                    {
                        await context.Response.WriteAsync($"data: {line}\n", cancellationToken).ConfigureAwait(false);
                    }

                    await context.Response.WriteAsync("\n", cancellationToken).ConfigureAwait(false);
                    await context.Response.Body.FlushAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Client disconnected — expected
            }
            finally
            {
                connectionManager.Disconnect(userId);
            }
        })
        .WithName("SseStream")
        .WithSummary("Server-Sent Events stream")
        .WithDescription("Real-time event stream. Requires authentication. Events are pushed as they occur.")
        .WithTags("SSE")
        .RequireAuthorization()
        .ExcludeFromDescription(); // Hide from OpenAPI since SSE doesn't map well to request/response

        return endpoints;
    }
}
