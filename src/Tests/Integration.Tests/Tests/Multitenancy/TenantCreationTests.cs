using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Multitenancy;

[Collection(FshCollectionDefinition.Name)]
public sealed class TenantCreationTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public TenantCreationTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task CreateTenant_Should_ReturnSuccess_When_DataIsValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            id = $"t-{uniqueId}",
            name = $"Test Tenant {uniqueId}",
            connectionString = (string?)null,
            adminEmail = $"admin-{uniqueId}@tenant.com",
            issuer = "test.issuer"
        };

        // Act
        var response = await client.PostAsJsonAsync(TestConstants.TenantsBasePath, payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await response.DeserializeAsync<CreateTenantResult>();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(payload.id);
    }

    [Fact]
    public async Task CreateTenant_Should_ReturnError_When_IdAlreadyExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            id = $"dup-{uniqueId}",
            name = $"Dup Tenant {uniqueId}",
            connectionString = (string?)null,
            adminEmail = $"dupadmin-{uniqueId}@tenant.com",
            issuer = "dup.issuer"
        };

        // Create first
        var firstResponse = await client.PostAsJsonAsync(TestConstants.TenantsBasePath, payload);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - create duplicate
        var secondResponse = await client.PostAsJsonAsync(TestConstants.TenantsBasePath, payload);

        // Assert
        secondResponse.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateTenant_Should_Return401_When_NotAuthenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", TestConstants.RootTenantId);
        var payload = new
        {
            id = "noauth",
            name = "No Auth Tenant",
            connectionString = (string?)null,
            adminEmail = "noauth@tenant.com",
            issuer = "noauth.issuer"
        };

        // Act
        var response = await client.PostAsJsonAsync(TestConstants.TenantsBasePath, payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTenants_Should_ReturnAllTenants_When_AuthenticatedAsRootAdmin()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.TenantsBasePath}?pageNumber=1&pageSize=50");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTenantStatus_Should_ReturnStatus_When_TenantExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.TenantsBasePath}/{TestConstants.RootTenantId}/status");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}

public sealed class CreateTenantResult
{
    public string Id { get; set; } = default!;
    public string? ProvisioningCorrelationId { get; set; }
    public string? Status { get; set; }
}
