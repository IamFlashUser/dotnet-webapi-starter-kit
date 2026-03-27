using Integration.Tests.Infrastructure;

namespace Integration.Tests.Tests.Health;

[Collection(FshCollectionDefinition.Name)]
public sealed class HealthCheckTests
{
    private readonly FshWebApplicationFactory _factory;

    public HealthCheckTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LivenessEndpoint_Should_Return200_When_AppIsRunning()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReadinessEndpoint_Should_Return200_When_AllDependenciesHealthy()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RootEndpoint_Should_Return200_When_AppIsRunning()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("hello world");
    }
}
