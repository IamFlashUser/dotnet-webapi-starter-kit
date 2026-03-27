using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Users;

[Collection(FshCollectionDefinition.Name)]
public sealed class UserManagementTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public UserManagementTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GetProfile_Should_ReturnCurrentUser_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/profile");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var user = await response.DeserializeAsync<UserDto>();
        user.Email.ShouldBe(TestConstants.RootAdminEmail, StringCompareShould.IgnoreCase);
    }

    [Fact]
    public async Task GetUsers_Should_ReturnPagedList_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/users?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnUser_When_UserExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // First register a user to get their ID
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var registerPayload = new
        {
            firstName = "GetById",
            lastName = "Test",
            email = $"getbyid-{uniqueId}@example.com",
            userName = $"getbyid-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var registerResult = await registerResponse.DeserializeAsync<RegisterResult>();

        // Act
        var response = await client.GetAsync($"{TestConstants.IdentityBasePath}/users/{registerResult.UserId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var user = await response.DeserializeAsync<UserDto>();
        user.ShouldNotBeNull();
        user.FirstName.ShouldBe("GetById");
        user.LastName.ShouldBe("Test");
    }

    [Fact]
    public async Task UpdateUser_Should_ModifyDetails_When_DataIsValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Register a user
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var registerPayload = new
        {
            firstName = "Before",
            lastName = "Update",
            email = $"update-{uniqueId}@example.com",
            userName = $"update-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var registerResult = await registerResponse.DeserializeAsync<RegisterResult>();

        // Act - update the user
        var updatePayload = new
        {
            firstName = "After",
            lastName = "Updated",
            phoneNumber = "+1234567890"
        };
        var response = await client.PutAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/users/{registerResult.UserId}", updatePayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the update
        var getResponse = await client.GetAsync($"{TestConstants.IdentityBasePath}/users/{registerResult.UserId}");
        var user = await getResponse.DeserializeAsync<UserDto>();
        user.FirstName.ShouldBe("After");
        user.LastName.ShouldBe("Updated");
    }

    [Fact]
    public async Task ToggleUserStatus_Should_DeactivateUser_When_IsActiveSetToFalse()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Register a user
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var registerPayload = new
        {
            firstName = "Toggle",
            lastName = "Status",
            email = $"toggle-{uniqueId}@example.com",
            userName = $"toggle-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var registerResult = await registerResponse.DeserializeAsync<RegisterResult>();

        // Act - deactivate the user
        var togglePayload = new
        {
            userId = registerResult.UserId,
            isActive = false
        };
        var response = await client.PostAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/toggle-user-status", togglePayload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteUser_Should_Return200_When_UserExists()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Register a user
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var registerPayload = new
        {
            firstName = "Delete",
            lastName = "Test",
            email = $"delete-{uniqueId}@example.com",
            userName = $"delete-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", registerPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
        var registerResult = await registerResponse.DeserializeAsync<RegisterResult>();

        // Act
        var response = await client.DeleteAsync($"{TestConstants.IdentityBasePath}/users/{registerResult.UserId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}

public sealed class UserDto
{
    public string Id { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageUrl { get; set; }
}

