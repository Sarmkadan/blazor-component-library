using Microsoft.Extensions.DependencyInjection;
using BlazorComponentLibrary;

namespace BlazorComponentLibrary.Examples;

/// <summary>
/// Basic setup and simple usage of the Blazor Component Library.
/// </summary>
public class BasicUsage
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register required services
        services.AddBlazorComponentLibrary();
    }

    public void TriggerToast(IToastService toastService)
    {
        // Simple notification
        toastService.Show("Task completed successfully.", ToastType.Success);
    }
}
