namespace Jerry.Aspire.Hosting.SqlServer;

public static class SqlDatabaseExtensions
{
    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, string tsql)
    {
        ArgumentException.ThrowIfNullOrEmpty(tsql);

        var startupDirectory = builder.DatabaseStartupDirectory(true);

        var filePath = Path.Combine(startupDirectory.FullName, "init.sql");

        File.WriteAllText(filePath, tsql);

        return builder;
    }

    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, params FileInfo[] files)
    {
        ArgumentOutOfRangeException.ThrowIfZero(files.Length, nameof(files));

        foreach (var file in files)
        {
            if (!file.Exists)
            {
                throw new Exception($"File [{file.FullName}] does not exist.");
            }
        }

        var startupDirectory = builder.DatabaseStartupDirectory(true);

        foreach (var file in files)
        {
            var destinationPath = Path.Combine(startupDirectory.FullName, file.Name);

            file.CopyTo(destinationPath, true);
        }

        return builder;
    }

    public static IResourceBuilder<SqlServerDatabaseResource> WithSqlScript(this IResourceBuilder<SqlServerDatabaseResource> builder, DirectoryInfo directory)
    {
        ArgumentNullException.ThrowIfNull(directory);

        if (!directory.Exists)
        {
            throw new Exception($"Directory [{directory.FullName}] does not exist.");
        }

        var files = directory.GetFiles().ToArray();

        if (files.Length == 0)
        {
            throw new Exception("No files found in directory.");
        }

        return builder.WithSqlScript(files);
    }
}