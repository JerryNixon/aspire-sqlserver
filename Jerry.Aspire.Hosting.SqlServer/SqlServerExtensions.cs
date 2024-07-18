using Aspire.Hosting;

using HealthChecks.SqlServer;

namespace Jerry.Aspire.Hosting.SqlServer;

public static class SqlServerExtensions
{
    public static IResourceBuilder<SqlServerServerResource> WithSqlPersistence(this IResourceBuilder<SqlServerServerResource> builder, bool cleanEveryTime = false)
    {
        var serverPath = builder.ServerRootDirectory();

        var data = Path.Combine(serverPath.FullName, "data");

        if (cleanEveryTime && Directory.Exists(data))
        {
            Directory.Delete(data, true);
            Directory.CreateDirectory(data);
        }

        var logs = Path.Combine(serverPath.FullName, "log");

        if (cleanEveryTime && Directory.Exists(logs))
        {
            Directory.Delete(logs, true);
            Directory.CreateDirectory(logs);
        }

        return builder
            .WithBindMount(data, "/var/opt/mssql/data")
            .WithBindMount(logs, "/var/opt/mssql/log");
    }

    public static IResourceBuilder<SqlServerServerResource> WithSqlScriptSupport(this IResourceBuilder<SqlServerServerResource> builder)
    {
        var shellDirectory = builder.ServerShellDirectory();

        if (shellDirectory.GetFiles().Length == 0)
        {
            new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "configure-db.sh"))
                .CopyTo(Path.Combine(shellDirectory.FullName, "configure-db.sh"));

            new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "configure-db.sh"))
                .CopyTo(Path.Combine(shellDirectory.FullName, "entrypoint.sh"));
        }

        var startDirectory = builder.ServerStartupDirectory();

        return builder
            .WithBindMount(shellDirectory.FullName, "/usr/config")
            .WithBindMount(startDirectory.FullName, "/docker-entrypoint-initdb.d")
            .WithEntrypoint("/usr/config/entrypoint.sh");
    }

    public static IResourceBuilder<SqlServerServerResource> WithHealthCheck(this IResourceBuilder<SqlServerServerResource> builder)
    {
        return builder.WithAnnotation(HealthCheckAnnotation.Create(cs =>
        {
            var options = new SqlServerHealthCheckOptions
            {
                ConnectionString = cs
            };
            return new SqlServerHealthCheck(options);
        }));
    }
}