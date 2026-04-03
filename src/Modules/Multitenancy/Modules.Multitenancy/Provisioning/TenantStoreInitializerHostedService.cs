using FSH.Framework.Shared.Multitenancy;
using FSH.Modules.Multitenancy.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FSH.Modules.Multitenancy.Provisioning;

/// <summary>
/// Initializes the tenant catalog database and seeds the root tenant.
/// Runs as a BackgroundService so it does NOT block the host from accepting requests.
/// </summary>
public sealed class TenantStoreInitializerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantStoreInitializerHostedService> _logger;

    public TenantStoreInitializerHostedService(
        IServiceProvider serviceProvider,
        ILogger<TenantStoreInitializerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();

            var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await tenantDbContext.Database.MigrateAsync(stoppingToken).ConfigureAwait(false);
            _logger.LogInformation("Applied tenant catalog migrations.");

            if (await tenantDbContext.TenantInfo.FindAsync([MultitenancyConstants.Root.Id], stoppingToken).ConfigureAwait(false) is null)
            {
                var rootTenant = new AppTenantInfo(
                    MultitenancyConstants.Root.Id,
                    MultitenancyConstants.Root.Name,
                    string.Empty,
                    MultitenancyConstants.Root.EmailAddress,
                    issuer: MultitenancyConstants.Root.Issuer);

                var validUpto = TimeProvider.System.GetUtcNow().UtcDateTime.AddYears(1);
                rootTenant.SetValidity(validUpto);
                await tenantDbContext.TenantInfo.AddAsync(rootTenant, stoppingToken).ConfigureAwait(false);
                await tenantDbContext.SaveChangesAsync(stoppingToken).ConfigureAwait(false);

                _logger.LogInformation("Seeded root tenant.");
            }
        }
        // Broad catch is intentional: tenant store initialization is critical but must not crash the host;
        // errors are logged and operations will fail with clear errors until resolved.
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Failed to initialize tenant catalog database. Tenant operations may fail until resolved.");
        }
    }
}
