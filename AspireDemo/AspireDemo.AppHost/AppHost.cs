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
 // Helt standard .NET EF Core migrasjonsservice f.eks.
 // For eksempel se :https://learn.microsoft.com/en-us/dotnet/aspire/database/ef-core-migrations?tabs=dotnet-cli%2Cdotnet-cli-2
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

// Legg til en Aspire worker som kjører hvert 5. minutt og henter v�rdata fra YR
var weatherWorker = builder.AddProject<Projects.AspireDemo_Worker>("weatherworker")
    .WaitFor(db)
    .WithReference(db)
    .WithEnvironment("FETCH_INTERVAL_MINUTES", "5");

var frontendPort = 3000;
var frontendUrl = "http://localhost:" + frontendPort;

/* eller man kan ha npm / pnpm / yarn app som kjører npm install + npm run dev */
// For andre typer, se https://learn.microsoft.com/en-us/dotnet/aspire/community-toolkit/hosting-nodejs-extensions?tabs=dotnet-cli%2Cyarn
/*
 * 
   var viteProsjekt = builder
       .AddPnpmApp("frontend", "../frontend", "dev")
       .WithReference(api)
       .WaitFor(api)
       .WithEnvironment("NODE_ENV", "development")
       .WithEnvironment("VITE_APP_URL", frontendUrl)
       .WithEnvironment("BROWSER", "none") // Prevents the browser from auto-opening
       .WithHttpEndpoint(frontendUrl, name: "frontend", env: "PORT")
       .WithPnpmPackageInstallation()
       .WithExternalHttpEndpoints();
   
   // Vi kunne skrevet om denne til å alltid kjøre _før_ frontend for å automatisk brekke ting ved endringer
   var frontendTypes = builder
       .AddPnpmApp("frontend-generate-typescript-api", "../frontend", "generate")
       .WithEnvironment("VITE_AUTH_API_URL", api.GetEndpoint("http"))
       .WithParentRelationship(frontend)
       .WithReference(api)
       .WaitFor(api)
       .WithExplicitStart();
 */

builder.Build().Run();
