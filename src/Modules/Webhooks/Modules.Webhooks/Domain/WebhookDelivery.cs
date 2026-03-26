namespace FSH.Modules.Webhooks.Domain;

public sealed class WebhookDelivery
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public Guid SubscriptionId { get; private set; }
    public string EventType { get; private set; } = default!;
    public string PayloadJson { get; private set; } = default!;
    public int HttpStatusCode { get; private set; }
    public bool Success { get; private set; }
    public int AttemptCount { get; private set; } = 1;
    public DateTime AttemptedAtUtc { get; private set; }
    public string? ErrorMessage { get; private set; }

    private WebhookDelivery() { }

    public static WebhookDelivery Create(Guid subscriptionId, string eventType, string payloadJson)
    {
        return new WebhookDelivery
        {
            SubscriptionId = subscriptionId,
            EventType = eventType,
            PayloadJson = payloadJson,
            AttemptedAtUtc = TimeProvider.System.GetUtcNow().UtcDateTime
        };
    }

    public void RecordResult(int statusCode, bool success, string? errorMessage)
    {
        HttpStatusCode = statusCode;
        Success = success;
        ErrorMessage = errorMessage;
    }
}
