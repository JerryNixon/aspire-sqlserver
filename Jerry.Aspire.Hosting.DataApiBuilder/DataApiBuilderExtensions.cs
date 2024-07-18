namespace Jerry.Aspire.Hosting.DataApiBuilder;

public static class DataApiBuilderExtensions
{
    public class DataApiBuilderResource : ContainerResource, IResourceWithConnectionString
    {
        public DataApiBuilderResource(string name, string? entrypoint = null) : base(name, entrypoint)
        {
            // empty
        }

        public ReferenceExpression ConnectionStringExpression => default!;
    }

    public static IResourceBuilder<DataApiBuilderResource> AddDataApiBuilder(this IDistributedApplicationBuilder builder, string name, int? httpPort = null, int? httpsPort = null)
    {
        (string Name, string Tag) image = ("mcr.microsoft.com/azure-databases/data-api-builder", "latest");

        var container = new DataApiBuilderResource(name);

        var result = builder.AddResource(container)
            .WithImage(image.Name, image.Tag)
            // .WithEnvironment("ASPNETCORE_URLS", "http://localhost:5000;http://localhost:5001;")
            .WithHttpEndpoint(5000, targetPort: 5000, name: "httpEndpoint");
        // .WithHttpsEndpoint(5001, targetPort: 5001, name: "httpsEndpoint");

        var targetDirectory = DataApiConfigDirectory(result, clearFirst: true);

        return result;
    }

    public static IResourceBuilder<DataApiBuilderResource> WithConnectionString(this IResourceBuilder<DataApiBuilderResource> builder, IResourceBuilder<IResourceWithConnectionString> source, string? connectionName = "my-connection-string")
    {
        ArgumentNullException.ThrowIfNull(connectionName, nameof(connectionName));

        return builder
            .WithEnvironment(connectionName, source.Resource.ConnectionStringExpression);
    }

    public static IResourceBuilder<DataApiBuilderResource> WithConfigurationFiles(this IResourceBuilder<DataApiBuilderResource> builder, params FileInfo[] configs)
    {
        ValidateArguments();

        var targetDirectory = DataApiConfigDirectory(builder, clearFirst: true);

        foreach (var config in configs)
        {
            builder.WithBindMount(config.FullName, $"/App/{config.Name}");
        }

        return builder;

        void ValidateArguments()
        {
            if (configs.Length == 0)
            {
                throw new ArgumentOutOfRangeException("Zero files provided.");
            }

            foreach (var config in configs)
            {
                if (!config.Exists)
                {
                    throw new FileNotFoundException("Configuration file not found.");
                }
            }
        }
    }

    private static DirectoryInfo DataApiConfigDirectory<T>(this IResourceBuilder<T> builder, bool clearFirst)
        where T : IResource
    {
        builder.EnsureGitIgnore();

        var serverDirectory = Path.GetFullPath($"data-api-builder/dab-{builder.Resource.Name}", builder.ApplicationBuilder.AppHostDirectory);

        if (Directory.Exists(serverDirectory) && clearFirst)
        {
            Directory.Delete(serverDirectory, true);
        }

        return Directory.CreateDirectory(serverDirectory);
    }

    private static void EnsureGitIgnore<T>(this IResourceBuilder<T> builder)
        where T : IResource
    {
        var rootDirectoryPath = builder.ApplicationBuilder.AppHostDirectory;

        var gitignore = Path.Combine(rootDirectoryPath, "../.gitignore");

        var exception = $"/data-api-builder";

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


