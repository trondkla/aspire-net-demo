using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var sqlServerHostPort = builder.Configuration.GetValue("SqlServerHostPort", 52677);
// List of first party integrations: https://learn.microsoft.com/en-us/dotnet/aspire/database/postgresql-integration?tabs=dotnet-cli
#pragma warning disable ASPIREPROXYENDPOINTS001
var sqlServer = builder.AddSqlServer("db")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithHostPort(sqlServerHostPort)
    .WithEndpointProxySupport(false);
#pragma warning restore ASPIREPROXYENDPOINTS001
var db = sqlServer.AddDatabase("DefaultConnection");

var cache = builder.AddRedis("cache");

/*
var migrations = builder.AddProject<Projects.AspireDemo_MigrationService>("migrations")
    .WithReference(db)
    .WaitFor(db);
*/

var apiService = builder.AddProject<Projects.AspireDemo_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(db)
    .WithReference(db)
    //.WithReference(migrations)
    //.WaitForCompletion(migrations)
    .WithEndpoint("http", endpoint => endpoint.IsProxied = false)
    ;

builder.AddProject<Projects.AspireDemo_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

// Legg til en Aspire worker som kjører hvert 5. minutt og henter værdata fra YR
var weatherWorker = builder.AddProject<Projects.AspireDemo_Worker>("weatherworker")
    .WaitFor(db)
    .WithReference(db)
    .WithEnvironment("FETCH_INTERVAL_MINUTES", "5");

builder.Build().Run();
