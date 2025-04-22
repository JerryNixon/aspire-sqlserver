using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Classroom.App;

static class Program
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static string? DataApiUrl = "http://localhost:1234";
    public static string? DataApiBuilderUrl = "services__data-api__http__0";

    static Program()
    {
        DataApiBuilderUrl = Environment.GetEnvironmentVariable(DataApiBuilderUrl);
    }

    [STAThread]
    static void Main()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLogging(config =>
        {
            config.AddConsole(); // or AddDebug()
            config.SetMinimumLevel(LogLevel.Information); // or Debug
        });

        if (string.IsNullOrWhiteSpace(DataApiBuilderUrl))
        {
            services.AddSingleton<IClassroomClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ClassroomApiClient>>();
                return new ClassroomApiClient(DataApiUrl!, logger);
            });
        }
        else
        {
            services.AddSingleton<IClassroomClient>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ClassroomDabClient>>();
                return new ClassroomDabClient(DataApiBuilderUrl!, logger);
            });
        }

        Services = services.BuildServiceProvider();

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
