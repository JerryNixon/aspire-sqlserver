var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password");
var sqlServer = builder
    .AddSqlServer("sql", sqlPassword, port: 1234)
    .WithDataVolume("MyDataVolume")
    .WithHealthCheck();
var sqlDatabase = sqlServer.AddDatabase("Database");

var sqlShell = "./sql-server";
var sqlScript = "../Database/sql";
sqlServer
    .WithBindMount(sqlShell, target: "/usr/config")
    .WithBindMount(sqlScript, target: "/docker-entrypoint-initdb.d")
    .WithEntrypoint("/usr/config/entrypoint.sh");

var dabConfig = "./data-api/dab-config.json";
var dabServer = builder
    .AddContainer("data-api", "mcr.microsoft.com/azure-databases/data-api-builder")
    .WithBindMount(dabConfig, "/App/dab-config.json")
    .WithHttpEndpoint(port: 5000, targetPort: 5000, name: "http")
    .WithReference(sqlDatabase)
    .WaitFor(sqlServer);

builder
    .AddProject<Projects.Client>("client")
    .WithEnvironment("DabUrl", dabServer.GetEndpoint("http"))
    .WaitFor(dabServer);

builder.Build().Run();
