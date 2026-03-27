using Integration.Tests.Infrastructure;
using Integration.Tests.Infrastructure.Extensions;

namespace Integration.Tests.Tests.Multitenancy;

[Collection(FshCollectionDefinition.Name)]
public sealed class TenantIsolationTests
{
    private readonly FshWebApplicationFactory _factory;
    private readonly AuthHelper _auth;

    public TenantIsolationTests(FshWebApplicationFactory factory)
    {
        _factory = factory;
        _auth = new AuthHelper(factory);
    }

    [Fact]
    public async Task TenantIsolation_Should_NotLeakUsers_When_QueryingAcrossTenants()
    {
        // Arrange
        using var rootClient = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create a new tenant
        var tenantId = $"iso-{uniqueId}";
        var tenantAdminEmail = $"isoadmin-{uniqueId}@tenant.com";
        var createPayload = new
        {
            id = tenantId,
            name = $"Isolation Tenant {uniqueId}",
            connectionString = (string?)null,
            adminEmail = tenantAdminEmail,
            issuer = "isolation.issuer"
        };
        var createResponse = await rootClient.PostAsJsonAsync(TestConstants.TenantsBasePath, createPayload);
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Wait for provisioning and try to login to the new tenant
        // The provisioning happens via Hangfire, so we may need to wait
        await WaitForProvisioningAsync(rootClient, tenantId);

        // Login as admin of new tenant
        using var tenantClient = await _auth.CreateAuthenticatedClientAsync(
            tenantAdminEmail,
            TestConstants.DefaultPassword,
            tenantId);

        // Create a user in the new tenant
        var userPayload = new
        {
            firstName = "Isolated",
            lastName = "User",
            email = $"isolated-{uniqueId}@example.com",
            userName = $"isolateduser-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var registerResponse = await tenantClient.PostAsJsonAsync(
            $"{TestConstants.IdentityBasePath}/register", userPayload);
        registerResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - query users from root tenant
        var rootUsersResponse = await rootClient.GetAsync(
            $"{TestConstants.IdentityBasePath}/users/search?pageNumber=1&pageSize=100");
        rootUsersResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var rootUsersContent = await rootUsersResponse.Content.ReadAsStringAsync();

        // Assert - the tenant-specific user should NOT appear in root tenant's user list
        rootUsersContent.ShouldNotContain($"isolated-{uniqueId}@example.com");
    }

    [Fact]
    public async Task TenantIsolation_Should_AllowSameEmailInDifferentTenants()
    {
        // Arrange
        using var rootClient = await _auth.CreateAuthenticatedClientAsync();
        var uniqueId = Guid.NewGuid().ToString("N")[..8];

        // Create two tenants
        var tenant1Id = $"dup1-{uniqueId}";
        var tenant2Id = $"dup2-{uniqueId}";
        string sharedEmail = $"shared-{uniqueId}@example.com";

        await CreateAndProvisionTenantAsync(rootClient, tenant1Id, uniqueId);
        await CreateAndProvisionTenantAsync(rootClient, tenant2Id, uniqueId);

        // Login to tenant 1 and create user
        using var client1 = await _auth.CreateAuthenticatedClientAsync(
            $"t1admin-{uniqueId}@tenant.com", TestConstants.DefaultPassword, tenant1Id);

        var userPayload = new
        {
            firstName = "Shared",
            lastName = "Email",
            email = sharedEmail,
            userName = $"shared1-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var register1 = await client1.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", userPayload);
        register1.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act - login to tenant 2 and create user with same email
        using var client2 = await _auth.CreateAuthenticatedClientAsync(
            $"t2admin-{uniqueId}@tenant.com", TestConstants.DefaultPassword, tenant2Id);

        var user2Payload = new
        {
            firstName = "Shared",
            lastName = "Email",
            email = sharedEmail,
            userName = $"shared2-{uniqueId}",
            password = "Test@1234!",
            confirmPassword = "Test@1234!"
        };
        var register2 = await client2.PostAsJsonAsync($"{TestConstants.IdentityBasePath}/register", user2Payload);

        // Assert - same email should be allowed in different tenants
        register2.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    private async Task CreateAndProvisionTenantAsync(HttpClient rootClient, string tenantId, string uniqueId)
    {
        string suffix = tenantId.StartsWith("dup1") ? "t1" : "t2";
        var payload = new
        {
            id = tenantId,
            name = $"Tenant {tenantId}",
            connectionString = (string?)null,
            adminEmail = $"{suffix}admin-{uniqueId}@tenant.com",
            issuer = $"{suffix}.issuer"
        };
        var response = await rootClient.PostAsJsonAsync(TestConstants.TenantsBasePath, payload);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        await WaitForProvisioningAsync(rootClient, tenantId);
    }

    private static async Task WaitForProvisioningAsync(HttpClient client, string tenantId, int maxRetries = 30)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            var statusResponse = await client.GetAsync(
                $"{TestConstants.TenantsBasePath}/{tenantId}/provisioning/status");

            if (statusResponse.IsSuccessStatusCode)
            {
                var content = await statusResponse.Content.ReadAsStringAsync();
                if (content.Contains("Success", StringComparison.OrdinalIgnoreCase)
                    || content.Contains("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            await Task.Delay(1000);
        }

        // If provisioning hasn't completed, continue anyway - some tests may still work
    }
}
