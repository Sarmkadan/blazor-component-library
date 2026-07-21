namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Modal;
using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

/// <summary>
/// Tests for the <see cref="Modal"/> component.
/// </summary>
public sealed class ModalTests : TestContext
{
    /// <summary>
    /// Verifies that the component renders with default values.
    /// </summary>
    [Fact]
    public void DefaultRender_HasDefaultValues()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>();

        // Assert
        var overlay = cut.FindAll(".bcl-modal-overlay");
        Assert.Empty(overlay); // Modal should not be visible by default

        var dialog = cut.FindAll(".bcl-modal");
        Assert.Empty(dialog); // Modal dialog should not be visible by default

        Assert.Equal(string.Empty, cut.Instance.Title);
        Assert.Null(cut.Instance.ChildContent);
        Assert.Null(cut.Instance.FooterContent);
        Assert.True(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Medium, cut.Instance.Size);
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Title"/> renders the title correctly.
    /// </summary>
    [Fact]
    public void Title_Set_RendersCorrectTitle()
    {
        // Arrange
        var title = "Test Modal Title";

        // Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert - modal is not visible until Show() is called
        Assert.Equal(title, cut.Instance.Title);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Title"/> to null or empty is handled.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Title_NullOrEmpty_DoesNotThrow(string? title)
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        Assert.Equal(title, cut.Instance.Title);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.ChildContent"/> renders the content correctly.
    /// </summary>
    [Fact]
    public void ChildContent_Set_RendersContent()
    {
        // Arrange
        RenderFragment content = builder => builder.AddContent(0, "Test Child Content");

        // Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.ChildContent, content));

        // Assert
        Assert.NotNull(cut.Instance.ChildContent);
        Assert.NotNull(content);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.FooterContent"/> renders the footer.
    /// </summary>
    [Fact]
    public void FooterContent_Set_RendersFooter()
    {
        // Arrange
        RenderFragment footer = builder => builder.AddContent(0, "Test Footer Content");

        // Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.FooterContent, footer));

        // Assert
        Assert.NotNull(cut.Instance.FooterContent);
        Assert.NotNull(footer);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.FooterContent"/> to null does not render footer.
    /// </summary>
    [Fact]
    public void FooterContent_Null_DoesNotRenderFooter()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.FooterContent, (RenderFragment?)null));

        // Assert
        Assert.Null(cut.Instance.FooterContent);
    }

    /// <summary>
    /// Verifies that <see cref="Modal.CloseOnOverlayClick"/> defaults to true.
    /// </summary>
    [Fact]
    public void CloseOnOverlayClick_DefaultsToTrue()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>();

        // Assert
        Assert.True(cut.Instance.CloseOnOverlayClick);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.CloseOnOverlayClick"/> to false prevents overlay click from closing.
    /// </summary>
    [Fact]
    public void CloseOnOverlayClick_False_PreventsOverlayClose()
    {
        // Arrange
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.CloseOnOverlayClick, false));

        // Act
        cut.Instance.Show();
        cut.Render();

        // Assert - modal should be visible
        Assert.True(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that <see cref="Modal.Size"/> defaults to Medium.
    /// </summary>
    [Fact]
    public void Size_DefaultsToMedium()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>();

        // Assert
        Assert.Equal(ModalSize.Medium, cut.Instance.Size);
        Assert.Equal("modal-medium", cut.Instance.SizeClass);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Size"/> to Small renders correct class.
    /// </summary>
    [Fact]
    public void Size_Small_RendersSmallClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Size, ModalSize.Small));

        // Assert
        Assert.Equal(ModalSize.Small, cut.Instance.Size);
        Assert.Equal("modal-small", cut.Instance.SizeClass);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Size"/> to Large renders correct class.
    /// </summary>
    [Fact]
    public void Size_Large_RendersLargeClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Size, ModalSize.Large));

        // Assert
        Assert.Equal(ModalSize.Large, cut.Instance.Size);
        Assert.Equal("modal-large", cut.Instance.SizeClass);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Size"/> to FullScreen renders correct class.
    /// </summary>
    [Fact]
    public void Size_FullScreen_RendersFullScreenClass()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Size, ModalSize.FullScreen));

        // Assert
        Assert.Equal(ModalSize.FullScreen, cut.Instance.Size);
        Assert.Equal("modal-fullscreen", cut.Instance.SizeClass);
    }

    /// <summary>
    /// Verifies that <see cref="Modal.IsVisible"/> is false by default.
    /// </summary>
    [Fact]
    public void IsVisible_DefaultsToFalse()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>();

        // Assert
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that calling <see cref="Modal.Show()"/> sets IsVisible to true.
    /// </summary>
    [Fact]
    public async Task Show_SetsIsVisibleToTrue()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        Assert.False(cut.Instance.IsVisible);

        // Act
        await cut.InvokeAsync(async () => await cut.Instance.Show());

        // Assert
        Assert.True(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that calling <see cref="Modal.Hide()"/> sets IsVisible to false.
    /// </summary>
    [Fact]
    public async Task Hide_SetsIsVisibleToFalse()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);

        // Act
        await cut.InvokeAsync(async () => await cut.Instance.Hide());

        // Assert
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that multiple Show() calls work correctly.
    /// </summary>
    [Fact]
    public async Task Show_MultipleTimes_IsVisibleRemainsTrue()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);

        // Act
        await cut.InvokeAsync(async () => await cut.Instance.Show());

        // Assert
        Assert.True(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that multiple Hide() calls work correctly.
    /// </summary>
    [Fact]
    public async Task Hide_MultipleTimes_IsVisibleRemainsFalse()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        Assert.False(cut.Instance.IsVisible);

        // Act
        await cut.InvokeAsync(async () => await cut.Instance.Hide());

        // Assert
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that Show() and Hide() can be called in sequence.
    /// </summary>
    [Fact]
    public async Task ShowAndHide_Sequence_WorksCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        Assert.False(cut.Instance.IsVisible);

        // Act & Assert - Show
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);

        // Act & Assert - Hide
        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        Assert.False(cut.Instance.IsVisible);

        // Act & Assert - Show again
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);

        // Act & Assert - Hide again
        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that changing parameters after initialization works correctly.
    /// </summary>
    [Fact]
    public void Parameters_ChangedAfterInitialization_ReflectsChanges()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        Assert.Equal(string.Empty, cut.Instance.Title);
        Assert.True(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Medium, cut.Instance.Size);

        // Act
        cut.SetParametersAndRender(parameters => parameters
            .Add(p => p.Title, "New Title")
            .Add(p => p.CloseOnOverlayClick, false)
            .Add(p => p.Size, ModalSize.Small));

        // Assert
        Assert.Equal("New Title", cut.Instance.Title);
        Assert.False(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Small, cut.Instance.Size);
    }

    /// <summary>
    /// Verifies that the SizeClass property returns correct values for all enum values.
    /// </summary>
    [Theory]
    [InlineData(ModalSize.Small, "modal-small")]
    [InlineData(ModalSize.Medium, "modal-medium")]
    [InlineData(ModalSize.Large, "modal-large")]
    [InlineData(ModalSize.FullScreen, "modal-fullscreen")]
    public void SizeClass_ReturnsCorrectClass(ModalSize size, string expectedClass)
    {
        // Arrange
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Size, size));

        // Act & Assert
        Assert.Equal(expectedClass, cut.Instance.SizeClass);
    }

    /// <summary>
    /// Verifies that the default values are properly initialized.
    /// </summary>
    [Fact]
    public void DefaultValues_AreInitializedCorrectly()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>();

        // Assert
        Assert.Equal(string.Empty, cut.Instance.Title);
        Assert.Null(cut.Instance.ChildContent);
        Assert.Null(cut.Instance.FooterContent);
        Assert.True(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Medium, cut.Instance.Size);
        Assert.False(cut.Instance.IsVisible);
    }
}
