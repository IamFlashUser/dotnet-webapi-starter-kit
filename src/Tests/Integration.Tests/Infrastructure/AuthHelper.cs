using System.Text.Json;

namespace Integration.Tests.Infrastructure;

public sealed class AuthHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly FshWebApplicationFactory _factory;

    public AuthHelper(FshWebApplicationFactory factory)
    {
        _factory = factory;
    }

    public async Task<TokenResult> GetRootAdminTokenAsync(CancellationToken ct = default)
    {
        // Root tenant provisioning runs asynchronously via Hangfire.
        // Retry login until the admin user is seeded and ready.
        const int maxRetries = 30;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await GetTokenAsync(
                    TestConstants.RootAdminEmail,
                    TestConstants.DefaultPassword,
                    TestConstants.RootTenantId,
                    ct).ConfigureAwait(false);
            }
            catch (HttpRequestException) when (i < maxRetries - 1)
            {
                await Task.Delay(1000, ct).ConfigureAwait(false);
            }
        }

        return await GetTokenAsync(
            TestConstants.RootAdminEmail,
            TestConstants.DefaultPassword,
            TestConstants.RootTenantId,
            ct).ConfigureAwait(false);
    }

    public async Task<TokenResult> GetTokenAsync(
        string email,
        string password,
        string tenant = "root",
        CancellationToken ct = default)
    {
        using var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestConstants.IdentityBasePath}/token/issue");
        request.Headers.Add("tenant", tenant);
        request.Content = JsonContent.Create(new { email, password });

        var response = await client.SendAsync(request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        var token = JsonSerializer.Deserialize<TokenResult>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize token response");
        return token;
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(
        string tenant = "root",
        CancellationToken ct = default)
    {
        var token = await GetRootAdminTokenAsync(ct).ConfigureAwait(false);
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
        client.DefaultRequestHeaders.Add("tenant", tenant);
        return client;
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(
        string email,
        string password,
        string tenant = "root",
        CancellationToken ct = default)
    {
        var token = await GetTokenAsync(email, password, tenant, ct).ConfigureAwait(false);
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);
        client.DefaultRequestHeaders.Add("tenant", tenant);
        return client;
    }
}

public sealed class TokenResult
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpiresAt { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
}
