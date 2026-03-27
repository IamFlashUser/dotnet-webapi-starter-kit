using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Roles;

[Collection(FshCollectionDefinition.Name)]
public sealed class RoleManagementTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public RoleManagementTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GetRoles_Should_ReturnSeededRoles_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/roles");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var roles = await response.DeserializeAsync<RoleDto[]>();
        roles.ShouldNotBeNull();
        roles.Length.ShouldBeGreaterThanOrEqualTo(2); // Admin + Basic roles
        roles.ShouldContain(r => r.Name == "Admin");
        roles.ShouldContain(r => r.Name == "Basic");
    }

    [Fact]
    public async Task CreateRole_Should_ReturnRole_When_NameIsUnique()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            id = string.Empty,
            name = $"TestRole-{uniqueId}",
            description = "A test role created by integration tests"
        };

        // Act
        var response = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/roles", payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var role = await response.DeserializeAsync<RoleDto>();
        role.ShouldNotBeNull();
        role.Name.ShouldBe(payload.name);
    }

    [Fact]
    public async Task DeleteRole_Should_Return200_When_RoleExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create a role first
        var createPayload = new
        {
            id = string.Empty,
            name = $"DeleteMe-{uniqueId}",
            description = "Role to be deleted"
        };
        var createResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/roles", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var createdRole = await createResponse.DeserializeAsync<RoleDto>();

        // Act
        var response = await client.DeleteAsync($"{TestConstants.IdentityBasePath}/roles/{createdRole.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRoles_Should_Return401_When_NotAuthenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", TestConstants.RootTenantId);

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/roles");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}

public sealed class RoleDto
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
