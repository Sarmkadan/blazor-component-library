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
/// Tests for the <see cref="ToastContainer"/> component's toast rendering behavior.
/// Tests show toast on service event, removes after dismiss, and multiple toasts order.
/// </summary>
public sealed class ToastContainerTests : TestContext
{
    [Fact]
    public void ShowsToast_WhenToastServiceRaisesToastsChangedEvent()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Assert - initially no toasts
        cut.FindAll(".bcl-toast").Count.Should().Be(0);
        
        // Act - show a toast
        service.Show("Test message", ToastType.Info, 4000);
        
        // Assert - toast should be visible
        var toasts = cut.FindAll(".bcl-toast");
        toasts.Count.Should().Be(1);
        toasts[0].TextContent.Should().Contain("Test message");
        toasts[0].TextContent.Should().Contain("ℹ"); // Info icon
    }

    [Fact]
    public void RemovesToast_AfterDismissEvent()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show a toast
        service.Show("Test message", ToastType.Info, 4000);
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(1));
        
        // Get the toast ID
        var toastId = service.ActiveToasts[0].Id;
        
        // Act - dismiss the toast
        service.Dismiss(toastId);
        
        // Assert - toast should be removed
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(0));
    }

    [Fact]
    public void RendersMultipleToasts_InOrder()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show multiple toasts
        service.Show("First message", ToastType.Info, 4000);
        service.Show("Second message", ToastType.Success, 4000);
        service.Show("Third message", ToastType.Warning, 4000);
        
        // Assert - all toasts should be visible
        var toasts = cut.FindAll(".bcl-toast");
        toasts.Count.Should().Be(3);
        
        // Check order (oldest first)
        toasts[0].TextContent.Should().Contain("First message");
        toasts[1].TextContent.Should().Contain("Second message");
        toasts[2].TextContent.Should().Contain("Third message");
    }

    [Fact]
    public void RendersCorrectToastIcons_ForDifferentTypes()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show toasts of different types
        service.Show("Info toast", ToastType.Info, 4000);
        service.Show("Success toast", ToastType.Success, 4000);
        service.Show("Warning toast", ToastType.Warning, 4000);
        service.Show("Error toast", ToastType.Error, 4000);
        
        // Assert - check icons
        var toasts = cut.FindAll(".bcl-toast");
        toasts.Count.Should().Be(4);
        
        toasts[0].TextContent.Should().Contain("ℹ"); // Info
        toasts[1].TextContent.Should().Contain("✓"); // Success
        toasts[2].TextContent.Should().Contain("⚠"); // Warning
        toasts[3].TextContent.Should().Contain("✕"); // Error
    }

    [Fact]
    public void ShowsDismissButton_ForEachToast()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show a toast
        service.Show("Test message", ToastType.Info, 4000);
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(1));
        
        // Assert - dismiss button should exist
        var dismissButtons = cut.FindAll(".bcl-toast__close");
        dismissButtons.Count.Should().Be(1);
        dismissButtons[0].TextContent.Should().Contain("✕");
    }

    [Fact]
    public void DismissesToast_WhenDismissButtonClicked()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show a toast
        service.Show("Test message", ToastType.Info, 4000);
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(1));
        
        // Get the dismiss button and click it
        var dismissButton = cut.Find(".bcl-toast__close");
        dismissButton.Click();
        
        // Assert - toast should be removed
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(0));
    }

    [Fact]
    public void RespectsMaxVisible_ShowingOnlyLastNToasts()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render with MaxVisible = 3
        var cut = RenderComponent<ToastContainer>(parameters => parameters
            .Add(p => p.MaxVisible, 3));
        
        // Show 5 toasts
        for (int i = 1; i <= 5; i++)
        {
            service.Show($"Message {i}", ToastType.Info, 4000);
        }
        
        // Assert - only last 3 should be visible
        cut.WaitForAssertion(() => {
            var toasts = cut.FindAll(".bcl-toast");
            toasts.Count.Should().Be(3);
            
            // Check that the last 3 are shown
            toasts[0].TextContent.Should().Contain("Message 3");
            toasts[1].TextContent.Should().Contain("Message 4");
            toasts[2].TextContent.Should().Contain("Message 5");
        });
    }

    [Fact]
    public void ShowsToastWithCustomIcon_WhenProvided()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show toast with custom icon
        service.Show("Custom icon toast", ToastType.Info, 4000, "🔥");
        
        // Assert - custom icon should be displayed
        var toasts = cut.FindAll(".bcl-toast");
        toasts.Count.Should().Be(1);
        toasts[0].TextContent.Should().Contain("🔥");
    }

    [Fact]
    public void DismissesAllToasts_WhenDismissAllCalled()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show multiple toasts
        service.Show("First", ToastType.Info, 4000);
        service.Show("Second", ToastType.Success, 4000);
        service.Show("Third", ToastType.Warning, 4000);
        
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(3));
        
        // Act - dismiss all
        service.DismissAll();
        
        // Assert - all toasts should be removed
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(0));
    }

    [Fact]
    public void ContainerHasCorrectAriaAttributes()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Assert - check ARIA attributes
        var container = cut.Find("div");
        container.GetAttribute("role").Should().Be("status");
        container.GetAttribute("aria-live").Should().Be("polite");
        container.GetAttribute("aria-atomic").Should().Be("false");
    }

    [Fact]
    public void ToastHasCorrectAriaAttributes()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show a toast
        service.Show("Accessible toast", ToastType.Info, 4000);
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(1));
        
        // Assert - check ARIA attributes on toast
        var toast = cut.Find(".bcl-toast");
        toast.GetAttribute("role").Should().Be("alert");
    }

    [Fact]
    public void ToastHasDismissButtonWithAriaLabel()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Act - render the component
        var cut = RenderComponent<ToastContainer>();
        
        // Show a toast
        service.Show("Test", ToastType.Info, 4000);
        cut.WaitForAssertion(() => cut.FindAll(".bcl-toast").Count.Should().Be(1));
        
        // Assert - check dismiss button aria-label
        var dismissButton = cut.Find(".bcl-toast__close");
        dismissButton.GetAttribute("aria-label").Should().Be("Dismiss notification");
    }

    [Fact]
    public void ShowsToastWithDifferentPositions()
    {
        // Arrange
        var service = new ToastService();
        Services.AddSingleton<IToastService>(service);
        
        // Test TopLeft position
        var cut = RenderComponent<ToastContainer>(parameters => parameters
            .Add(p => p.Position, ToastPosition.TopLeft));
        
        service.Show("TopLeft toast", ToastType.Info, 4000);
        cut.WaitForAssertion(() => {
            var container = cut.Find("div");
            container.ClassList.Should().Contain("bcl-toast-container--top-left");
        });
        
        // Test BottomRight position (default)
        cut = RenderComponent<ToastContainer>(parameters => parameters
            .Add(p => p.Position, ToastPosition.BottomRight));
        
        service.Show("BottomRight toast", ToastType.Info, 4000);
        cut.WaitForAssertion(() => {
            var container = cut.Find("div");
            container.ClassList.Should().Contain("bcl-toast-container--bottom-right");
        });
    }
}
