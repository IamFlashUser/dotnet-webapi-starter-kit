namespace Integration.Tests.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    public static HttpClient WithTenant(this HttpClient client, string tenant)
    {
        client.DefaultRequestHeaders.Remove("tenant");
        client.DefaultRequestHeaders.Add("tenant", tenant);
        return client;
    }

    public static HttpClient WithAuth(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }
}
