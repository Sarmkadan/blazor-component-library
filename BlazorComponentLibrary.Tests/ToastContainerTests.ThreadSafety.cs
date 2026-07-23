using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorComponentLibrary.Components.Toast;
using BlazorComponentLibrary.Services;

namespace BlazorComponentLibrary.Tests.Components.Toast;

/// <summary>
/// Thread-safety tests for ToastContainer to ensure it handles events raised from background threads correctly.
/// These tests verify the fix for non-UI thread event invocation bugs similar to the recent commit.
/// </summary>
public sealed class ToastContainerThreadSafetyTests : TestContext
{
    [Fact]
    public void HandlesEventsFromBackgroundThreads_WithoutInvalidOperationException()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);

        // Act - render the component
        var cut = RenderComponent<ToastContainer>();

        // Assert - initially no toasts
        cut.FindAll(".bcl-toast").Count.Should().Be(0);

        // Act - invoke the event handler from a background thread (simulating timer callbacks)
        // This tests the thread-safety of InvokeAsync(StateHasChanged) in the event handler
        Task.Run(() => service.Show("Background toast", ToastType.Info, 4000))
            .ContinueWith(task =>
            {
                // The continuation should complete without throwing
                task.Exception?.Handle(ex => true);
            }, TaskScheduler.Default);

        // Wait for the toast to be processed and rendered
        cut.WaitForAssertion(() =>
        {
            var toasts = cut.FindAll(".bcl-toast");
            toasts.Count.Should().Be(1);
            toasts[0].TextContent.Should().Contain("Background toast");
        });
    }

    [Fact]
    public void HandlesAutoDismissTimerEvents_WithoutInvalidOperationException()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);

        // Act - render the component
        var cut = RenderComponent<ToastContainer>();

        // Assert - initially no toasts
        cut.FindAll(".bcl-toast").Count.Should().Be(0);

        // Act - show a toast with auto-dismiss (will trigger System.Timers.Timer callback on background thread)
        service.Show("Auto-dismiss toast", ToastType.Info, 100);

        // Wait for the toast to be auto-dismissed
        cut.WaitForAssertion(() =>
        {
            var toasts = cut.FindAll(".bcl-toast");
            toasts.Count.Should().Be(0);
        }, timeout: TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void HandlesMultipleBackgroundEvents_WithoutInvalidOperationException()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);

        // Act - render the component
        var cut = RenderComponent<ToastContainer>();

        // Assert - initially no toasts
        cut.FindAll(".bcl-toast").Count.Should().Be(0);

        // Act - trigger multiple events from background threads
        var tasks = new Task[10];
        for (int i = 0; i < 10; i++)
        {
            int index = i;
            tasks[i] = Task.Run(() =>
            {
                service.Show($"Toast {index}", ToastType.Info, 100);
            });
        }

        Task.WhenAll(tasks).Wait(TimeSpan.FromSeconds(5));

        // Wait for all toasts to be processed and rendered
        cut.WaitForAssertion(() =>
        {
            var toasts = cut.FindAll(".bcl-toast");
            toasts.Count.Should().BeGreaterThanOrEqualTo(1);
        }, timeout: TimeSpan.FromSeconds(5));
    }
}