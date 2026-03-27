using System.Reflection;
using FSH.Framework.Jobs.Services;
using FSH.Framework.Shared.Persistence;
using FSH.Framework.Web.Modules;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Testcontainers.PostgreSql;

namespace Integration.Tests.Infrastructure;

public sealed class FshWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine")
        .WithDatabase("fsh_integration_tests")
        .WithUsername("postgres")
        .WithPassword("integration_test_pwd")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        ResetModuleLoader();

        // Trigger host creation by creating a client.
        // The host starts, running all hosted services including
        // TenantStoreInitializerHostedService and TenantAutoProvisioningHostedService.
        using var client = CreateClient();

        // Manually run all IDbInitializer instances to ensure migrations and seeding
        // are complete. Hangfire InMemory may process jobs slowly in tests.
        using var scope = Services.CreateScope();
        var initializers = scope.ServiceProvider.GetServices<IDbInitializer>();
        foreach (var init in initializers)
        {
            await init.MigrateAsync(CancellationToken.None);
            await init.SeedAsync(CancellationToken.None);
        }
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DatabaseOptions:Provider"] = "POSTGRESQL",
                ["DatabaseOptions:ConnectionString"] = _postgres.GetConnectionString(),
                ["DatabaseOptions:MigrationsAssembly"] = "FSH.Migrations.PostgreSQL",
                ["CachingOptions:Redis"] = "",
                ["JwtOptions:Issuer"] = TestConstants.JwtIssuer,
                ["JwtOptions:Audience"] = TestConstants.JwtAudience,
                ["JwtOptions:SigningKey"] = TestConstants.JwtSigningKey,
                ["JwtOptions:AccessTokenMinutes"] = "30",
                ["JwtOptions:RefreshTokenDays"] = "7",
                ["OriginOptions:OriginUrl"] = "http://localhost",
                ["MultitenancyOptions:RunTenantMigrationsOnStartup"] = "true",
                ["MultitenancyOptions:AutoProvisionOnStartup"] = "true",
                ["OpenTelemetryOptions:Enabled"] = "false",
                ["Serilog:MinimumLevel:Default"] = "Warning",
                ["Serilog:WriteTo:0:Name"] = "Console",
                ["Serilog:WriteTo:0:Args:restrictedToMinimumLevel"] = "Warning",
                ["Serilog:WriteTo:1:Name"] = "",
                ["MailOptions:UseSendGrid"] = "false",
                ["HangfireOptions:Username"] = "admin",
                ["HangfireOptions:Password"] = "admin",
                ["HangfireOptions:Route"] = "/jobs",
                ["RateLimitingOptions:Enabled"] = "false",
                ["PasswordPolicy:EnforcePasswordExpiry"] = "false",
                ["SecurityHeadersOptions:Enabled"] = "false",
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace Hangfire PostgreSQL storage with InMemory
            services.AddHangfire(config => config.UseInMemoryStorage());

            // Ensure IJobService is registered
            services.TryAddTransient<IJobService, HangfireService>();

            // Override JWT bearer to not require HTTPS (TestServer uses HTTP)
            services.PostConfigure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options => options.RequireHttpsMetadata = false);
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseServiceProviderFactory(new DefaultServiceProviderFactory(new ServiceProviderOptions
        {
            ValidateOnBuild = false,
            ValidateScopes = false
        }));

        ResetModuleLoader();
        return base.CreateHost(builder);
    }

    private static void ResetModuleLoader()
    {
        var type = typeof(ModuleLoader);
        var modulesField = type.GetField("_modules", BindingFlags.Static | BindingFlags.NonPublic);
        var loadedField = type.GetField("_modulesLoaded", BindingFlags.Static | BindingFlags.NonPublic);

        if (modulesField?.GetValue(null) is System.Collections.IList list)
        {
            list.Clear();
        }

        loadedField?.SetValue(null, false);
    }
}
