public static class SqlServerExtensions
{
    public static IResourceBuilder<SqlServerDatabaseResource> WithVsCodeSupport(this IResourceBuilder<SqlServerDatabaseResource> builder)
    {
        return builder; 
    }

    public static IResourceBuilder<SqlServerServerResource> WithSqlDataMount(this IResourceBuilder<SqlServerServerResource> builder, bool cleanEveryTime = false)
    {
        builder.EnsureGitIgnore();

        return builder
            .WithBindMount(EnsureDirectory("data").FullName, "/var/opt/mssql/data")
            .WithBindMount(EnsureDirectory("log").FullName, "/var/opt/mssql/log");

        DirectoryInfo EnsureDirectory(string moniker)
        {
            var serverPath = ServerDirectory(builder).FullName;

            var newPath = Path.Combine(serverPath, moniker);

            if (cleanEveryTime && Directory.Exists(newPath))
            {
                Directory.Delete(newPath, true);
            }

            return Directory.CreateDirectory(newPath);
        }
    }

    public static IResourceBuilder<SqlServerServerResource> WithSqlScriptSupport(this IResourceBuilder<SqlServerServerResource> builder)
    {
        builder.EnsureGitIgnore();

        var serverDirectory = builder.ServerDirectory();

        var shellDirectory = Path.Combine(serverDirectory.FullName, "shell");

        Directory.CreateDirectory(shellDirectory);

        if (Directory.GetFiles(shellDirectory).Length == 0)
        {
            AddFile(
                target: Path.Combine(shellDirectory, "configure-db.sh"),
                source: new Uri("https://raw.githubusercontent.com/JerryNixon/aspire-sqlserver/main/shell/configure-db.sh"));
            AddFile(
                target: Path.Combine(shellDirectory, "entrypoint.sh"),
                source: new Uri("https://raw.githubusercontent.com/JerryNixon/aspire-sqlserver/main/shell/entrypoint.sh"));
        }

        var sqlDirectory = Path.Combine(serverDirectory.FullName, "sql");

        Directory.CreateDirectory(sqlDirectory);

        return builder
            .WithBindMount(shellDirectory, "/usr/config")
            .WithBindMount(sqlDirectory, "/docker-entrypoint-initdb.d")
            .WithEntrypoint("/usr/config/entrypoint.sh");

        void AddFile(string target, Uri source)
        {
            using HttpClient client = new HttpClient();
            var content = client.GetStringAsync(source).Result;
            File.WriteAllText(target, content);
        }
    }

    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, string tsql)
    {
        ArgumentException.ThrowIfNullOrEmpty(tsql);

        builder.EnsureGitIgnore();

        var sqlDirectory = SqlDirectory(builder, true);

        var filePath = Path.Combine(sqlDirectory.FullName, "init.sql");

        File.WriteAllText(filePath, tsql);

        return builder;
    }

    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (!file.Exists)
        {
            throw new Exception($"File [{file.FullName}] does not exist.");
        }

        builder.EnsureGitIgnore();

        var sqlDirectory = SqlDirectory(builder, true);

        var destinationPath = Path.Combine(sqlDirectory.FullName, file.Name);

        file.CopyTo(destinationPath, true);

        return builder;
    }

    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        if (!directory.Exists)
        {
            throw new Exception($"Directory [{directory.FullName}] does not exist.");
        }

        var files = directory.GetFiles();

        if (files.Length == 0)
        {
            throw new Exception("No files found in directory.");
        }

        builder.EnsureGitIgnore();

        var sqlDirectory = SqlDirectory(builder, true);

        foreach (var file in files)
        {
            var destination = Path.Combine(sqlDirectory.FullName, file.Name);
            file.CopyTo(destination);
        }

        return builder;
    }

    private static DirectoryInfo ServerDirectory(this IResourceBuilder<SqlServerServerResource> builder)
    {
        builder.EnsureGitIgnore();

        var serverDirectory = Path.GetFullPath($"sqlserver/server-{builder.Resource.Name}", builder.ApplicationBuilder.AppHostDirectory);

        return Directory.CreateDirectory(serverDirectory);
    }

    private static DirectoryInfo SqlDirectory(this IResourceBuilder<SqlServerDatabaseResource> builder, bool clearFiles)
    {
        builder.EnsureGitIgnore();

        var serverDirectory = Path.GetFullPath($"sqlserver/server-{builder.Resource.Parent.Name}", builder.ApplicationBuilder.AppHostDirectory);

        var sqlDirectory = Path.Combine(serverDirectory, "sql", $"db-{builder.Resource.Name}");

        if (clearFiles && Directory.Exists(sqlDirectory))
        {
            Directory.Delete(sqlDirectory, true);
        }

        return Directory.CreateDirectory(sqlDirectory);
    }

    private static void EnsureGitIgnore<T>(this IResourceBuilder<T> builder)
        where T : IResourceWithConnectionString
    {
        var rootDirectoryPath = builder.ApplicationBuilder.AppHostDirectory;

        var gitignore = Path.Combine(rootDirectoryPath, ".gitignore");

        var exception = $"/sqlserver";

        if (File.Exists(gitignore) && !File.ReadAllLines(gitignore).Contains(exception))
        {
            File.AppendAllText(gitignore, $"{Environment.NewLine}{exception}");
        }
        else
        {
            File.WriteAllText(gitignore, exception);
        }
    }
}
