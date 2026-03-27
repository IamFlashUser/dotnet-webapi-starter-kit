using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Groups;

[Collection(FshCollectionDefinition.Name)]
public sealed class GroupMembershipTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public GroupMembershipTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task AddUsersToGroup_Should_Succeed_When_UsersExist()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create a user
        var registerPayload = new
        {
            firstName = "GroupMember",
            lastName = "Test",
            email = $"member-{uniqueId}@example.com",
            userName = $"member-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var user = await registerResponse.DeserializeAsync<Users.RegisterResult>();

        // Create a group
        var groupPayload = new
        {
            name = $"MemberGroup-{uniqueId}",
            description = "Group for membership tests",
            isDefault = false,
            roleIds = new List<string>()
        };
        var groupResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", groupPayload);
        groupResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var group = await groupResponse.DeserializeAsync<GroupDto>();

        // Act - add user to group
        var addPayload = new { userIds = new[] { user.UserId } };
        var response = await client.PostAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/groups/{group.Id}/members", addPayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroupMembers_Should_ReturnMembers_When_GroupHasMembers()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create a user and group, then add user to group
        var registerPayload = new
        {
            firstName = "GroupQuery",
            lastName = "Test",
            email = $"grpquery-{uniqueId}@example.com",
            userName = $"grpquery-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        var user = await registerResponse.DeserializeAsync<Users.RegisterResult>();

        var groupPayload = new
        {
            name = $"QueryGroup-{uniqueId}",
            description = "Group for querying members",
            isDefault = false,
            roleIds = new List<string>()
        };
        var groupResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", groupPayload);
        var group = await groupResponse.DeserializeAsync<GroupDto>();

        // Add member
        var addPayload = new { userIds = new[] { user.UserId } };
        await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups/{group.Id}/members", addPayload);

        // Act
        var response = await client.GetAsync(
            $"{TestConstants.IdentityBasePath}/groups/{group.Id}/members?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveUserFromGroup_Should_Succeed_When_UserIsInGroup()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create user + group + add member
        var registerPayload = new
        {
            firstName = "RemoveMe",
            lastName = "Test",
            email = $"remove-{uniqueId}@example.com",
            userName = $"remove-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        var user = await registerResponse.DeserializeAsync<Users.RegisterResult>();

        var groupPayload = new
        {
            name = $"RemoveGroup-{uniqueId}",
            description = "Group for removal test",
            isDefault = false,
            roleIds = new List<string>()
        };
        var groupResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/groups", groupPayload);
        var group = await groupResponse.DeserializeAsync<GroupDto>();

        await client.PostAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/groups/{group.Id}/members",
            new { userIds = new[] { user.UserId } });

        // Act
        var response = await client.DeleteAsync(
            $"{TestConstants.IdentityBasePath}/groups/{group.Id}/members/{user.UserId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
