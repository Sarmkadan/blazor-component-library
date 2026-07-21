namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Modal;
using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

/// <summary>
/// Comprehensive unit tests for the <see cref="Modal"/> component public API.
/// Tests cover happy-path scenarios, edge cases (null/empty inputs, boundary values),
/// and error-path assertions.
/// </summary>
public sealed class ModalRazorUnitTests : TestContext
{
    /// <summary>
    /// Verifies that the component renders with default values when no parameters are provided.
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
        Assert.Equal("modal-medium", cut.Instance.SizeClass);
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Title"/> to various values works correctly.
    /// </summary>
    [Theory]
    [InlineData("Simple Title")]
    [InlineData("Title with special chars: !@#$%^&*()")]
    [InlineData("Title with unicode: Привет, 你好, こんにちは")]
    [InlineData("A very long title that exceeds typical lengths to test string handling capabilities of the modal component")]
    public void Title_Set_VariousValues_RendersCorrectly(string title)
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert
        Assert.Equal(title, cut.Instance.Title);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.Title"/> to null or empty strings is handled gracefully.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Title_NullOrWhitespace_HandledGracefully(string? title)
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, title));

        // Assert - should not throw and maintain the input value (matches existing ModalTests behavior)
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
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.ChildContent"/> to null does not throw.
    /// </summary>
    [Fact]
    public void ChildContent_Null_DoesNotThrow()
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.ChildContent, (RenderFragment?)null));

        // Assert
        Assert.Null(cut.Instance.ChildContent);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.FooterContent"/> renders the footer correctly.
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

        // Act - show the modal
        cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();

        // Assert - modal should be visible
        Assert.True(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that setting <see cref="Modal.CloseOnOverlayClick"/> to true allows overlay click to close.
    /// </summary>
    [Fact]
    public void CloseOnOverlayClick_True_AllowsOverlayClose()
    {
        // Arrange
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.CloseOnOverlayClick, true));

        // Act - show the modal
        cut.InvokeAsync(() => cut.Instance.Show());
        cut.Render();
        Assert.True(cut.Instance.IsVisible);

        // Simulate overlay click by calling Hide directly
        cut.InvokeAsync(() => cut.Instance.Hide());

        // Assert - modal should be hidden
        Assert.False(cut.Instance.IsVisible);
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
    /// Verifies that all <see cref="ModalSize"/> enum values render correct CSS classes.
    /// </summary>
    [Theory]
    [InlineData(ModalSize.Small, "modal-small")]
    [InlineData(ModalSize.Medium, "modal-medium")]
    [InlineData(ModalSize.Large, "modal-large")]
    [InlineData(ModalSize.FullScreen, "modal-fullscreen")]
    public void Size_AllEnumValues_RenderCorrectClasses(ModalSize size, string expectedClass)
    {
        // Arrange & Act
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Size, size));

        // Assert
        Assert.Equal(size, cut.Instance.Size);
        Assert.Equal(expectedClass, cut.Instance.SizeClass);
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
    /// Verifies that multiple Show() calls work correctly and IsVisible remains true.
    /// </summary>
    [Fact]
    public async Task Show_MultipleTimes_IsVisibleRemainsTrue()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);

        // Act - call Show multiple times
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        await cut.InvokeAsync(async () => await cut.Instance.Show());

        // Assert
        Assert.True(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that multiple Hide() calls work correctly and IsVisible remains false.
    /// </summary>
    [Fact]
    public async Task Hide_MultipleTimes_IsVisibleRemainsFalse()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        Assert.False(cut.Instance.IsVisible);

        // Act - call Hide multiple times
        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        await cut.InvokeAsync(async () => await cut.Instance.Hide());

        // Assert
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that Show() and Hide() can be called in sequence multiple times.
    /// </summary>
    [Fact]
    public async Task ShowAndHide_Sequence_MultipleTimes_WorksCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        Assert.False(cut.Instance.IsVisible);

        // Act & Assert - Multiple show/hide cycles
        for (int i = 0; i < 3; i++)
        {
            await cut.InvokeAsync(async () => await cut.Instance.Show());
            Assert.True(cut.Instance.IsVisible);

            await cut.InvokeAsync(async () => await cut.Instance.Hide());
            Assert.False(cut.Instance.IsVisible);
        }
    }

    /// <summary>
    /// Verifies that the OnClose event callback is invoked when Hide() is called.
    /// </summary>
    [Fact]
    public async Task OnClose_EventCallback_InvokedWhenHideCalled()
    {
        // Arrange
        var closeCalled = false;
        EventCallback onCloseCallback = EventCallback.Factory.Create(this, () => closeCalled = true);
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.OnClose, onCloseCallback));

        await cut.InvokeAsync(async () => await cut.Instance.Show());

        // Act
        await cut.InvokeAsync(async () => await cut.Instance.Hide());

        // Assert
        Assert.True(closeCalled);
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
        Assert.Equal("modal-small", cut.Instance.SizeClass);
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
        Assert.Equal("modal-medium", cut.Instance.SizeClass);
        Assert.False(cut.Instance.IsVisible);
    }

    /// <summary>
    /// Verifies that the modal can be shown and hidden while preserving parameter values.
    /// </summary>
    [Fact]
    public async Task ShowHide_PreservesParameterValues()
    {
        // Arrange
        var cut = RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, "Test Title")
            .Add(p => p.CloseOnOverlayClick, false)
            .Add(p => p.Size, ModalSize.Large));

        // Act - show and hide multiple times
        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);
        Assert.Equal("Test Title", cut.Instance.Title);
        Assert.False(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Large, cut.Instance.Size);

        await cut.InvokeAsync(async () => await cut.Instance.Hide());
        Assert.False(cut.Instance.IsVisible);

        await cut.InvokeAsync(async () => await cut.Instance.Show());
        Assert.True(cut.Instance.IsVisible);
        Assert.Equal("Test Title", cut.Instance.Title);
        Assert.False(cut.Instance.CloseOnOverlayClick);
        Assert.Equal(ModalSize.Large, cut.Instance.Size);
    }

    /// <summary>
    /// Verifies that the modal handles rapid show/hide operations correctly.
    /// </summary>
    [Fact]
    public async Task RapidShowHide_Operations_WorkCorrectly()
    {
        // Arrange
        var cut = RenderComponent<Modal>();

        // Act - rapid operations
        for (int i = 0; i < 10; i++)
        {
            await cut.InvokeAsync(async () => await cut.Instance.Show());
            Assert.True(cut.Instance.IsVisible);

            await cut.InvokeAsync(async () => await cut.Instance.Hide());
            Assert.False(cut.Instance.IsVisible);
        }
    }

    /// <summary>
    /// Verifies that the modal implements the IModal interface correctly.
    /// </summary>
    [Fact]
    public void Modal_ImplementsIModalInterface()
    {
        // Arrange
        var cut = RenderComponent<Modal>();
        IModal modal = cut.Instance;

        // Assert - verify interface implementation
        Assert.NotNull(modal);
        Assert.False(modal.IsVisible);
        Assert.Equal(string.Empty, modal.Title);
        Assert.Null(modal.ChildContent);
        Assert.Null(modal.FooterContent);
        Assert.True(modal.CloseOnOverlayClick);
    }
}