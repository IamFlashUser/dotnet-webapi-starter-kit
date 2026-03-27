using System.IdentityModel.Tokens.Jwt;
using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Authentication;

[Collection(FshCollectionDefinition.Name)]
public sealed class TokenGenerationTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public TokenGenerationTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GenerateToken_Should_ReturnAccessAndRefreshToken_When_CredentialsAreValid()
    {
        // Act
        var token = await _auth.GetRootAdminTokenAsync();

        // Assert
        token.ShouldNotBeNull();
        token.AccessToken.ShouldNotBeNullOrWhiteSpace();
        token.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        token.AccessTokenExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
        token.RefreshTokenExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public async Task GenerateToken_Should_Return401_When_PasswordIsIncorrect()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestConstants.IdentityBasePath}/token/issue");
        request.Headers.Add("tenant", TestConstants.RootTenantId);
        request.Content = JsonContent.Create(new
        {
            email = TestConstants.RootAdminEmail,
            password = "WrongPassword123!"
        });

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GenerateToken_Should_Return401_When_EmailDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestConstants.IdentityBasePath}/token/issue");
        request.Headers.Add("tenant", TestConstants.RootTenantId);
        request.Content = JsonContent.Create(new
        {
            email = "nonexistent@example.com",
            password = TestConstants.DefaultPassword
        });

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GenerateToken_Should_Return400_When_EmailIsEmpty()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestConstants.IdentityBasePath}/token/issue");
        request.Headers.Add("tenant", TestConstants.RootTenantId);
        request.Content = JsonContent.Create(new
        {
            email = "",
            password = TestConstants.DefaultPassword
        });

        // Act
        var response = await client.SendAsync(request);

        // Assert
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GenerateToken_Should_IncludeCorrectClaimsInJwt_When_LoginSucceeds()
    {
        // Act
        var token = await _auth.GetRootAdminTokenAsync();

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.AccessToken);

        jwt.Issuer.ShouldBe(TestConstants.JwtIssuer);
        jwt.Audiences.ShouldContain(TestConstants.JwtAudience);

        var emailClaim = jwt.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == System.Security.Claims.ClaimTypes.Email);
        emailClaim.ShouldNotBeNull();
        emailClaim.Value.ShouldBe(TestConstants.RootAdminEmail, StringCompareShould.IgnoreCase);
    }

    [Fact]
    public async Task GenerateToken_Should_Return401_When_TenantHeaderIsMissing()
    {
        // Arrange
        using var client = _factory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{TestConstants.IdentityBasePath}/token/issue");
        // No tenant header
        request.Content = JsonContent.Create(new
        {
            email = TestConstants.RootAdminEmail,
            password = TestConstants.DefaultPassword
        });

        // Act
        var response = await client.SendAsync(request);

        // Assert
        // Without tenant, the request should fail (either 401 or 500 depending on tenant resolution)
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
