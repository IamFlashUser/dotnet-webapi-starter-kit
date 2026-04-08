using Aspire.Hosting.ApplicationModel;

var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("fsh-postgres-data")
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("fsh");

var redis = builder.AddRedis("redis")
    .WithDataVolume("fsh-redis-data")
    .WithLifetime(ContainerLifetime.Persistent);

// Build a plain TCP (non-TLS) Redis connection string using the secondary endpoint.
// Aspire 13.x enables TLS on the primary Redis port by default; the secondary endpoint is plain TCP.
var redisPlainTcp = redis.GetEndpoint("secondary");
var redisConnectionString = ReferenceExpression.Create(
    $"{redisPlainTcp.Property(EndpointProperty.HostAndPort)},password={redis.Resource.PasswordParameter!}");

// API Service
var api = builder.AddProject<Projects.FSH_Starter_Api>("fsh-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithExternalHttpEndpoints()
    .WithEnvironment("DatabaseOptions__Provider", "POSTGRESQL")
    .WithEnvironment("DatabaseOptions__ConnectionString", postgres.Resource.ConnectionStringExpression)
    .WithEnvironment("DatabaseOptions__MigrationsAssembly", "FSH.Starter.Migrations.PostgreSQL")
    .WithEnvironment("CachingOptions__Redis", redisConnectionString)
    .WithEnvironment("CachingOptions__EnableSsl", "false");

// Admin App (Next.js)
builder.AddJavaScriptApp("fsh-admin", "../../clients/admin", "dev")
    .WithNpm()
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(port: 3000, env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("FSH_API_URL", api.GetEndpoint("http"));

await builder.Build().RunAsync();
