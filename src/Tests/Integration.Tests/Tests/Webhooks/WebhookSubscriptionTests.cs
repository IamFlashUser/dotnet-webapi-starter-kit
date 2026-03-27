using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Webhooks;

[Collection(FshCollectionDefinition.Name)]
public sealed class WebhookSubscriptionTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public WebhookSubscriptionTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task CreateWebhookSubscription_Should_ReturnId_When_DataIsValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var payload = new
        {
            url = "https://example.com/webhook",
            events = new[] { "user.created", "user.updated" },
            secret = "test-webhook-secret-123"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{TestConstants.WebhooksBasePath}/subscriptions", payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetWebhookSubscriptions_Should_ReturnList_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Create a subscription first
        var createPayload = new
        {
            url = "https://example.com/webhook-list",
            events = new[] { "tenant.created" },
            secret = "list-secret-123"
        };
        await client.PostAsJsonAsync($"{TestConstants.WebhooksBasePath}/subscriptions", createPayload);

        // Act
        var response = await client.GetAsync(
            $"{TestConstants.WebhooksBasePath}/subscriptions?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteWebhookSubscription_Should_Return200_When_SubscriptionExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var createPayload = new
        {
            url = "https://example.com/webhook-delete",
            events = new[] { "user.deleted" },
            secret = "delete-secret-123"
        };
        var createResponse = await client.PostAsJsonAsync(
            $"{TestConstants.WebhooksBasePath}/subscriptions", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var subscriptionId = await createResponse.Content.ReadAsStringAsync();

        // Clean up the quotes from JSON string
        subscriptionId = subscriptionId.Trim('"');

        // Act
        var response = await client.DeleteAsync(
            $"{TestConstants.WebhooksBasePath}/subscriptions/{subscriptionId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateWebhookSubscription_Should_Return401_When_NotAuthenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", TestConstants.RootTenantId);
        var payload = new
        {
            url = "https://example.com/noauth-webhook",
            events = new[] { "user.created" },
            secret = "noauth-secret"
        };

        // Act
        var response = await client.PostAsJsonAsync(
            $"{TestConstants.WebhooksBasePath}/subscriptions", payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
