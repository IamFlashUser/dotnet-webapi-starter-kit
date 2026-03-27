using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Groups;

[Collection(FshCollectionDefinition.Name)]
public sealed class GroupCrudTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public GroupCrudTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GetGroups_Should_ReturnSeededGroups_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/groups?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateGroup_Should_Return201_When_DataIsValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            name = $"TestGroup-{uniqueId}",
            description = "Integration test group",
            isDefault = false,
            roleIds = new List<string>()
        };

        // Act
        var response = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var group = await response.DeserializeAsync<GroupDto>();
        group.ShouldNotBeNull();
        group.Name.ShouldBe(payload.name);
    }

    [Fact]
    public async Task UpdateGroup_Should_ModifyName_When_GroupExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create group first
        var createPayload = new
        {
            name = $"UpdateMe-{uniqueId}",
            description = "To be updated",
            isDefault = false,
            roleIds = new List<string>()
        };
        var createResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createdGroup = await createResponse.DeserializeAsync<GroupDto>();

        // Act
        var updatePayload = new
        {
            name = $"Updated-{uniqueId}",
            description = "Updated description",
            isDefault = false,
            roleIds = new List<string>()
        };
        var response = await client.PutAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/groups/{createdGroup.Id}", updatePayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteGroup_Should_Return200_When_GroupIsNotSystemGroup()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create group first
        var createPayload = new
        {
            name = $"DeleteMe-{uniqueId}",
            description = "To be deleted",
            isDefault = false,
            roleIds = new List<string>()
        };
        var createResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var createdGroup = await createResponse.DeserializeAsync<GroupDto>();

        // Act
        var response = await client.DeleteAsync($"{TestConstants.IdentityBasePath}/groups/{createdGroup.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}

public sealed class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDefault { get; set; }
    public bool IsSystemGroup { get; set; }
    public int MemberCount { get; set; }
}
