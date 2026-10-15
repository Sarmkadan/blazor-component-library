using System.Linq;
using Bunit;
using BlazorComponentLibrary.Components.Toast;
using BlazorComponentLibrary.Extensions;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests that valid <see cref="ToastOptions"/> registered through <c>AddBlazorComponentLibrary</c>
/// are applied to the toasts and container that are rendered.
/// </summary>
public sealed class ToastOptionsRenderTests : TestContext
{
    /// <summary>
    /// Verifies that a configured <see cref="ToastOptions.Position"/> is applied to the rendered container
    /// when no <c>Position</c> parameter is supplied.
    /// </summary>
    [Fact]
    public void RenderedContainer_ConfiguredPosition_AppliesPositionClass()
    {
        // Arrange
        Services.AddBlazorComponentLibrary(null, options => options.Position = ToastPosition.TopLeft);

        // Act
        var cut = RenderComponent<ToastContainer>();

        // Assert
        var container = cut.Find(".bcl-toast-container");
        Assert.Contains("bcl-toast-container--top-left", container.ClassList);
    }

    /// <summary>
    /// Verifies that an explicitly supplied <c>Position</c> parameter overrides the configured default.
    /// </summary>
    [Fact]
    public void RenderedContainer_ExplicitPositionParameter_OverridesConfiguredPosition()
    {
        // Arrange
        Services.AddBlazorComponentLibrary(null, options => options.Position = ToastPosition.TopLeft);

        // Act
        var cut = RenderComponent<ToastContainer>(p => p.Add(c => c.Position, ToastPosition.BottomCenter));

        // Assert
        var container = cut.Find(".bcl-toast-container");
        Assert.Contains("bcl-toast-container--bottom-center", container.ClassList);
    }

    /// <summary>
    /// Verifies that a configured <see cref="ToastOptions.MaxVisibleToasts"/> limits the rendered toasts
    /// to the newest ones.
    /// </summary>
    [Fact]
    public void RenderedContainer_ConfiguredMaxVisibleToasts_RendersOnlyNewestToasts()
    {
        // Arrange
        Services.AddBlazorComponentLibrary(null, options => options.MaxVisibleToasts = 2);
        var cut = RenderComponent<ToastContainer>();
        var toastService = Services.GetRequiredService<IToastService>();

        // Act
        toastService.Show("First", ToastType.Info, 0);
        toastService.Show("Second", ToastType.Info, 0);
        toastService.Show("Third", ToastType.Info, 0);

        // Assert
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindAll(".bcl-toast").Count));
        var messages = cut.FindAll(".bcl-toast__message").Select(m => m.TextContent.Trim()).ToList();
        Assert.Equal(new[] { "Second", "Third" }, messages);

        toastService.DismissAll();
    }

    /// <summary>
    /// Verifies that a toast shown without an explicit duration is rendered and auto-dismisses after the
    /// configured <see cref="ToastOptions.DefaultDuration"/>.
    /// </summary>
    [Fact]
    public void RenderedToast_ShownWithoutDuration_UsesConfiguredDefaultDuration()
    {
        // Arrange
        Services.AddBlazorComponentLibrary(null, options => options.DefaultDuration = TimeSpan.FromMilliseconds(1500));
        var cut = RenderComponent<ToastContainer>();
        var toastService = Services.GetRequiredService<IToastService>();

        // Act
        toastService.Show("Default duration toast");

        // Assert
        cut.WaitForAssertion(() => Assert.Single(cut.FindAll(".bcl-toast")));
        Assert.Equal(1500, toastService.ActiveToasts.Single().DurationMs);

        toastService.DismissAll();
    }

    /// <summary>
    /// Verifies that an explicit duration passed to <c>Show</c> overrides the configured default.
    /// </summary>
    [Fact]
    public void RenderedToast_ShownWithExplicitDuration_OverridesConfiguredDefaultDuration()
    {
        // Arrange
        Services.AddBlazorComponentLibrary(null, options => options.DefaultDuration = TimeSpan.FromSeconds(2));
        var toastService = Services.GetRequiredService<IToastService>();

        // Act
        toastService.Show("Explicit duration toast", ToastType.Info, 750);

        // Assert
        Assert.Equal(750, toastService.ActiveToasts.Single().DurationMs);

        toastService.DismissAll();
    }

    /// <summary>
    /// Verifies that hovering over a rendered toast pauses its timer when <see cref="ToastOptions.PauseOnHover"/>
    /// is left at its default of <c>true</c>.
    /// </summary>
    [Fact]
    public void RenderedToast_DefaultPauseOnHover_PausesTimerOnMouseOver()
    {
        // Arrange
        var toast = new ToastMessage(Guid.NewGuid(), "Hover me", ToastType.Info, 4000);
        var toastService = CreateToastServiceMock(toast);
        Services.AddSingleton(toastService.Object);
        Services.AddBlazorComponentLibrary(null);
        var cut = RenderComponent<ToastContainer>();

        // Act
        cut.Find(".bcl-toast").MouseOver();

        // Assert
        toastService.Verify(s => s.PauseTimer(toast.Id), Times.Once);
    }

    /// <summary>
    /// Verifies that hovering over a rendered toast does not pause its timer when
    /// <see cref="ToastOptions.PauseOnHover"/> is configured as <c>false</c>.
    /// </summary>
    [Fact]
    public void RenderedToast_PauseOnHoverDisabled_DoesNotPauseTimerOnMouseOver()
    {
        // Arrange
        var toast = new ToastMessage(Guid.NewGuid(), "Hover me", ToastType.Info, 4000);
        var toastService = CreateToastServiceMock(toast);
        Services.AddSingleton(toastService.Object);
        Services.AddBlazorComponentLibrary(null, options => options.PauseOnHover = false);
        var cut = RenderComponent<ToastContainer>();

        // Act
        cut.Find(".bcl-toast").MouseOver();

        // Assert
        toastService.Verify(s => s.PauseTimer(It.IsAny<Guid>()), Times.Never);
    }

    private static Mock<IToastService> CreateToastServiceMock(ToastMessage toast)
    {
        var toastService = new Mock<IToastService>();
        toastService.Setup(s => s.ActiveToasts).Returns(new List<ToastMessage> { toast }.AsReadOnly());
        return toastService;
    }
}
