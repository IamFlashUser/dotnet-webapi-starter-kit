using Integration.Tests.Infrastructure;

namespace Integration.Tests.Tests.Auditing;

[Collection(FshCollectionDefinition.Name)]
public sealed class AuditTrailTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public AuditTrailTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task GetAudits_Should_ReturnAuditEntries_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.AuditsBasePath}?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSecurityAudits_Should_ReturnEntries_When_LoginEventsExist()
    {
        // Arrange
        // Perform a login to generate security audit entries
        await _auth.GetRootAdminTokenAsync();

        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync(
            $"{TestConstants.AuditsBasePath}/security?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAuditSummary_Should_ReturnSummary_When_Authenticated()
    {
        // Arrange
        using var client = await _auth.CreateAuthenticatedClientAsync();

        // Act
        var response = await client.GetAsync($"{TestConstants.AuditsBasePath}/summary");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAudits_Should_Return401_When_NotAuthenticated()
    {
        // Arrange
        using var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("tenant", TestConstants.RootTenantId);

        // Act
        var response = await client.GetAsync($"{TestConstants.AuditsBasePath}?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
