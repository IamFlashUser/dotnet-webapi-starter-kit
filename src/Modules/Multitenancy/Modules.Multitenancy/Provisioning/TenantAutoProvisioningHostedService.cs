using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Multitenancy;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Modules.Multitenancy.Provisioning;

/// <summary>
/// Auto-provisions tenants that haven't completed provisioning.
/// Runs as a BackgroundService so it does NOT block the host from accepting requests.
/// Includes a brief delay to allow the tenant store initializer and Hangfire to start first.
/// </summary>
public sealed class TenantAutoProvisioningHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantAutoProvisioningHostedService> _logger;
    private readonly MultitenancyOptions _options;

    public TenantAutoProvisioningHostedService(
        IServiceProvider serviceProvider,
        ILogger<TenantAutoProvisioningHostedService> logger,
        IOptions<MultitenancyOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!ShouldRunProvisioning())
        {
            return;
        }

        // Wait briefly for the tenant store initializer to complete first
        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken).ConfigureAwait(false);

        if (!WaitForJobStorage(stoppingToken))
        {
            _logger.LogWarning("Hangfire storage not initialized; skipping auto-provisioning enqueue.");
            return;
        }

        try
        {
            await ProvisionTenantsAsync(stoppingToken).ConfigureAwait(false);
        }
        // Broad catch is intentional: auto-provisioning is best-effort on startup;
        // tenants can be provisioned manually if this fails.
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Auto-provisioning failed. Tenants may need manual provisioning.");
        }
    }

    private bool ShouldRunProvisioning() =>
        _options.AutoProvisionOnStartup || _options.RunTenantMigrationsOnStartup;

    private async Task ProvisionTenantsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();
        var provisioning = scope.ServiceProvider.GetRequiredService<ITenantProvisioningService>();

        var tenants = await tenantStore.GetAllAsync().ConfigureAwait(false);

        foreach (var tenant in tenants)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            await TryProvisionTenantAsync(provisioning, tenant, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task TryProvisionTenantAsync(ITenantProvisioningService provisioning, AppTenantInfo tenant, CancellationToken cancellationToken)
    {
        try
        {
            if (await ShouldProvisionTenantAsync(provisioning, tenant.Id, cancellationToken).ConfigureAwait(false))
            {
                await provisioning.StartAsync(tenant.Id, cancellationToken).ConfigureAwait(false);
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Enqueued provisioning for tenant {TenantId} on startup.", tenant.Id);
                }
            }
        }
        catch (CustomException ex)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Provisioning already in progress or recently queued for tenant {TenantId}: {Message}", tenant.Id, ex.Message);
            }
        }
        // Per-tenant failure must not stop provisioning of other tenants
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue provisioning for tenant {TenantId}", tenant.Id);
        }
    }

    private async Task<bool> ShouldProvisionTenantAsync(ITenantProvisioningService provisioning, string tenantId, CancellationToken cancellationToken)
    {
        if (_options.RunTenantMigrationsOnStartup)
        {
            return true;
        }

        var latest = await provisioning.GetLatestAsync(tenantId, cancellationToken).ConfigureAwait(false);
        return latest is null || latest.Status != TenantProvisioningStatus.Completed;
    }

    private static bool WaitForJobStorage(CancellationToken cancellationToken)
    {
        // Retry a few times since Hangfire may still be initializing
        for (int i = 0; i < 5; i++)
        {
            if (cancellationToken.IsCancellationRequested) return false;
            try
            {
                _ = JobStorage.Current;
                return true;
            }
            catch (InvalidOperationException)
            {
                Thread.Sleep(500);
            }
        }

        return false;
    }
}
