using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Multitenancy;

[Collection(FshCollectionDefinition.Name)]
public sealed class TenantActivationTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public TenantActivationTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task ChangeTenantActivation_Should_DeactivateTenant_When_TenantIsActive()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create a tenant first
        var createPayload = new
        {
            id = $"deact-{uniqueId}",
            name = $"Deactivate Tenant {uniqueId}",
            connectionString = (string?)null,
            adminEmail = $"deact-{uniqueId}@tenant.com",
            issuer = "deact.issuer"
        };
        var createResponse = await client.PostAsJsonAsync(TestConstants.TenantsBasePath, createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - deactivate
        var deactivatePayload = new { isActive = false };
        var response = await client.PutAsJsonAsync(
            $"{TestConstants.TenantsBasePath}/{createPayload.id}/activate", deactivatePayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ChangeTenantActivation_Should_ReactivateTenant_When_TenantIsDeactivated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create + deactivate
        var createPayload = new
        {
            id = $"react-{uniqueId}",
            name = $"Reactivate Tenant {uniqueId}",
            connectionString = (string?)null,
            adminEmail = $"react-{uniqueId}@tenant.com",
            issuer = "react.issuer"
        };
        await client.PostAsJsonAsync(TestConstants.TenantsBasePath, createPayload);
        await client.PutAsJsonAsync(
            $"{TestConstants.TenantsBasePath}/{createPayload.id}/activate", new { isActive = false });

        // Act - reactivate
        var response = await client.PutAsJsonAsync(
            $"{TestConstants.TenantsBasePath}/{createPayload.id}/activate", new { isActive = true });

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
