using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Roles;

[Collection(FshCollectionDefinition.Name)]
public sealed class RolePermissionTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public RolePermissionTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GetRolePermissions_Should_ReturnPermissions_When_RoleExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Get roles to find Admin role
        var rolesResponse = await client.GetAsync($"{TestConstants.IdentityBasePath}/roles");
        var roles = await rolesResponse.DeserializeAsync<RoleDto[]>();
        var adminRole = roles.First(r => r.Name == "Admin");

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/roles/{adminRole.Id}/permissions");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateRolePermissions_Should_AssignPermissions_When_PermissionsAreValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Create a new role
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var createPayload = new
        {
            id = string.Empty,
            name = $"PermRole-{uniqueId}",
            description = "Role for permission testing"
        };
        var createResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/roles", createPayload);
        var createdRole = await createResponse.DeserializeAsync<RoleDto>();

        // Act - assign permissions
        var permPayload = new
        {
            roleId = createdRole.Id,
            permissions = new[] { "Permissions.AuditTrails.View" }
        };
        var response = await client.PutAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/roles/{createdRole.Id}/permissions", permPayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
