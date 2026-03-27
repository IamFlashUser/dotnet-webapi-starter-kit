using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Users;

[Collection(FshCollectionDefinition.Name)]
public sealed class UserRegistrationTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public UserRegistrationTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task RegisterUser_Should_Return201_When_DataIsValid()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            firstName = "Test",
            lastName = "User",
            email = $"test-{uniqueId}@example.com",
            userName = $"testuser-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };

        // Act
        var response = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Register failed with {response.StatusCode}: {errorBody}");
        }

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await response.DeserializeAsync<RegisterResult>();
        result.UserId.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task RegisterUser_Should_Return400_When_EmailIsDuplicate()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            firstName = "Dup",
            lastName = "User",
            email = $"dup-{uniqueId}@example.com",
            userName = $"dupuser-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };

        // Register first time
        var firstResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);
        firstResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - register with same email again
        payload = payload with { userName = $"dupuser2-{uniqueId}" };
        var secondResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);

        // Assert
        secondResponse.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task RegisterUser_Should_ReturnError_When_PasswordTooWeak()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        var payload = new
        {
            firstName = "Weak",
            lastName = "Pass",
            email = $"weak-{uniqueId}@example.com",
            userName = $"weakuser-{uniqueId}",
            password = "123",
            confirmPassword = "123"
        };

        // Act
        var response = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task RegisterUser_Should_Return401_When_NotAuthenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", TestConstants.RootTenantId);
        var payload = new
        {
            firstName = "NoAuth",
            lastName = "User",
            email = "noauth@example.com",
            userName = "noauthuser",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };

        // Act
        var response = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegisterUser_Should_AllowLoginAfterRegistration()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        string email = $"login-{uniqueId}@example.com";
        string password = "Test@1234!";
        var payload = new
        {
            firstName = "Login",
            lastName = "Test",
            email,
            userName = $"loginuser-{uniqueId}",
            password,
            confirmPassword = password
        };

        var registerResponse = await client.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", payload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - login with newly created user
        var token = await _auth.GetTokenAsync(email, password);

        // Assert
        token.ShouldNotBeNull();
        token.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }
}

public sealed class RegisterResult
{
    public string UserId { get; set; } = default!;
}
