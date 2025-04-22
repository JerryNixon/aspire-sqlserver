var builder = DistributedApplication.CreateBuilder(args);

var sqlDatabase = builder.AddConnectionString("SqlServer");

var dabConfig = "./dab-config.json";
var dabApi = builder.AddDataAPIBuilder("data-api", dabConfig)
    .WithHttpHealthCheck("/api/Classes")
    .WithReference(sqlDatabase);

builder.AddProject<Projects.Classroom_App>("App")
    .WithReference(dabApi)
    .WaitFor(dabApi);

builder.Build().Run();

/*
var sqlServer = builder
    .AddSqlServer("sql-server", password: sqlPassword, port: 1234);
var sqlDatabase = sqlServer
    .AddDatabase("sql-database");
var dbProject = builder
    .AddSqlProject<Projects.Database>("db-project")
    .WithReference(sqlDatabase);
var dabConfig = "./dab-config.json";
var dabApi = builder
    .AddDataAPIBuilder("data-api", dabConfig)
    .WithReference(sqlDatabase)
    .WaitForCompletion(dbProj);
*/