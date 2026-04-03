using FSH.Framework.Shared.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace FSH.Framework.Jobs;

/// <summary>
/// Best-effort cleanup of stale Hangfire locks from crashed instances.
/// Runs as a BackgroundService so it never blocks application startup.
/// </summary>
internal sealed class HangfireStaleLockCleanupService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<HangfireStaleLockCleanupService> _logger;

    public HangfireStaleLockCleanupService(
        IConfiguration configuration,
        ILogger<HangfireStaleLockCleanupService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Short delay to let Hangfire initialize its schema first
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).ConfigureAwait(false);

        var dbOptions = _configuration.GetSection(nameof(DatabaseOptions)).Get<DatabaseOptions>();
        if (dbOptions is null || !dbOptions.Provider.Equals(DbProviders.PostgreSQL, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            await using var connection = new NpgsqlConnection(dbOptions.ConnectionString);
            await connection.OpenAsync(stoppingToken).ConfigureAwait(false);

            await using var cmd = new NpgsqlCommand(
                "DELETE FROM hangfire.lock WHERE acquired < NOW() - INTERVAL '5 minutes'",
                connection);

            int deleted = await cmd.ExecuteNonQueryAsync(stoppingToken).ConfigureAwait(false);
            if (deleted > 0)
            {
                _logger.LogWarning("Cleaned up {Count} stale Hangfire locks", deleted);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogDebug(ex, "Could not cleanup stale Hangfire locks (table may not exist yet)");
        }
    }
}
