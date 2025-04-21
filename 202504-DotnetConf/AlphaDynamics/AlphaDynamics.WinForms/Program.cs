using AlphaDynamics.WinForms;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

ApplicationConfiguration.Initialize();

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient<ApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:1234");
        });

        services.AddScoped<Form1>();
    });

using var host = builder.Build();
var form = host.Services.GetRequiredService<Form1>();
Application.Run(form);
