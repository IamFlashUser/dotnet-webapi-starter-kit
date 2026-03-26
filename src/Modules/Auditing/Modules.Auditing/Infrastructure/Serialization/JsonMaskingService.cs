using FSH.Modules.Auditing.Contracts;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FSH.Modules.Auditing;

/// <summary>
/// Simple masking by field-name convention or attributes.
/// </summary>
public sealed class JsonMaskingService : IAuditMaskingService
{
    private static readonly HashSet<string> MaskKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "password", "secret", "token", "otp", "pin",
        "accessToken", "refreshToken", "apiKey", "clientSecret",
        "authCode", "authorization", "bearer", "connectionString"
    };

    private const string MaskValue = "****";

    public object ApplyMasking(object payload)
    {
        try
        {
            var json = JsonSerializer.SerializeToNode(payload);
            if (json is null) return payload;
            MaskNode(json);
            return json;
        }
        catch (JsonException)
        {
            return payload; // safe fallback — payload is not valid JSON
        }
    }

    private static void MaskNode(JsonNode node)
    {
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj.ToList())
            {
                if (ShouldMask(kvp.Key))
                {
                    obj[kvp.Key] = MaskValue;
                }
                else if (kvp.Value is not null)
                {
                    MaskNode(kvp.Value);
                }
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var el in arr)
                if (el is not null) MaskNode(el);
        }
    }

    private static bool ShouldMask(string key)
        => MaskKeywords.Any(k => key.Contains(k, StringComparison.OrdinalIgnoreCase));
}

