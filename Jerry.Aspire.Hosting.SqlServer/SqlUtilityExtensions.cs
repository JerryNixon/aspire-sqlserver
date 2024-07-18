namespace Jerry.Aspire.Hosting.SqlServer;

internal static class SqlUtilityExtensions
{
    public static DirectoryInfo AppRootDirectory(this IResourceBuilder<IResourceWithConnectionString> builder)
    {
        var appRootPath = Path.GetFullPath("sql-server", builder.ApplicationBuilder.AppHostDirectory);

        EnsureGitIgnore(appRootPath);

        return Directory.CreateDirectory(appRootPath);

        static void EnsureGitIgnore(string path)
        {
            var gitignore = Path.Combine(path, "../.gitignore");

            var exception = $"/sql-server";

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

    public static DirectoryInfo ServerRootDirectory(this IResourceBuilder<IResourceWithConnectionString> builder)
    {
        var rootDirectory = builder.AppRootDirectory();

        var serverName = builder.Resource switch
        {
            SqlServerDatabaseResource db => db.Parent.Name,
            SqlServerServerResource server => server.Name,
            _ => throw new ArgumentOutOfRangeException(nameof(builder.Resource))
        };

        var serverPath = Path.Combine(rootDirectory.FullName, serverName);

        return Directory.CreateDirectory(serverPath);
    }

    public static DirectoryInfo ServerShellDirectory(this IResourceBuilder<IResourceWithConnectionString> builder)
    {
        var serverDirectory = builder.ServerRootDirectory();

        var shellPath = Path.Combine(serverDirectory.FullName, "shell");

        return Directory.CreateDirectory(shellPath);
    }

    public static DirectoryInfo ServerStartupDirectory(this IResourceBuilder<IResourceWithConnectionString> builder)
    {
        var rootDirectory = builder.ServerRootDirectory();

        var startupPath = Path.Combine(rootDirectory.FullName, "startup");

        return Directory.CreateDirectory(startupPath);
    }

    public static DirectoryInfo DatabaseStartupDirectory(this IResourceBuilder<SqlServerDatabaseResource> builder, bool clearFiles)
    {
        var serverDirectory = builder.ServerStartupDirectory();

        var startupPath = Path.Combine(serverDirectory.FullName, builder.Resource.Name);

        if (clearFiles && Directory.Exists(startupPath))
        {
            Directory.Delete(startupPath, true);
        }

        return Directory.CreateDirectory(startupPath);
    }
}