using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorComponentLibrary.Extensions;

namespace BlazorComponentLibrary.Examples;

/// <summary>
/// Shows how to integrate the library into an ASP.NET Core DI container.
/// </summary>
public class IntegrationExample
{
    public static void ConfigureHost(IHostApplicationBuilder builder)
    {
        // 1. Add services to container
        builder.Services.AddBlazorComponentLibrary();

        // 2. Add other application services
        // builder.Services.AddScoped<MyAppDataService>();
    }
}
