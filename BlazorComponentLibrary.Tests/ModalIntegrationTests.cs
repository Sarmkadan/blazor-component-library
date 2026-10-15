using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Modal;
using Microsoft.AspNetCore.Components;

namespace BlazorComponentLibrary.Tests;

public sealed class ModalIntegrationTests : TestContext
{
    /// <summary>
    /// Host component that wraps the Modal and provides a button to toggle it.
    /// </summary>
    private class ModalHost : ComponentBase
    {
        private bool _isVisible;
        private int _onCloseCallCount;
        private Modal? _modal;

        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.OpenElement(1, "button");
            builder.AddAttribute(2, "id", "open-modal-button");
            builder.AddAttribute(3, "onclick", EventCallback.Factory.Create(this, OpenModal));
            builder.AddContent(4, "Open Modal");
            builder.CloseElement(); // button

            builder.OpenComponent<Modal>(5);
            builder.AddAttribute(6, "Title", "Test Modal");
            builder.AddAttribute(7, "ChildContent", (RenderFragment)(builder2 =>
            {
                builder2.OpenElement(11, "div");
                builder2.AddAttribute(12, "id", "modal-content");
                builder2.AddContent(13, "Modal Body");
                builder2.CloseElement(); // div
            }));
            builder.AddAttribute(8, "FooterContent", (RenderFragment)(builder2 =>
            {
                builder2.OpenElement(15, "div");
                builder2.AddAttribute(16, "id", "modal-footer");
                builder2.AddContent(17, "Modal Footer");
                builder2.CloseElement(); // div
            }));
            builder.AddAttribute(9, "OnClose", EventCallback.Factory.Create(this, () => _onCloseCallCount++));
            // Capture reference to the modal component
            builder.AddComponentReferenceCapture(10, (modal) => _modal = (Modal?)modal);
            builder.CloseComponent(); // Modal

            builder.CloseElement(); // div
        }

        private void OpenModal()
        {
            if (_modal != null)
            {
                _modal.Show();
            }
        }

        public Modal? Modal => _modal;
        public int OnCloseCallCount => _onCloseCallCount;
    }

    [Fact]
    public void Modal_OpenCloseLifecycle_ViaButtonClick()
    {
        // Arrange
        var host = RenderComponent<ModalHost>();
        var openButton = host.Find("#open-modal-button");

        // Act - Open modal by clicking button (which calls modal.Show())
        openButton.Click();

        // Assert - Modal is visible
        var overlay = host.Find(".bcl-modal-overlay");
        var dialog = host.Find(".bcl-modal");
        Assert.NotNull(overlay);
        Assert.NotNull(dialog);
        Assert.Contains("Modal Body", host.Markup);
        Assert.Contains("Modal Footer", host.Markup);
        Assert.True(host.Instance.Modal!.IsVisible);

        // Verify initial callback count (should be 0 before any close action)
        Assert.Equal(0, host.Instance.OnCloseCallCount);

        // Act - Close modal via close button in header
        var closeButton = host.Find(".bcl-modal-close");
        closeButton.Click();

        // Assert - Modal is closed
        Assert.DoesNotContain(".bcl-modal-overlay", host.Markup);
        Assert.DoesNotContain(".bcl-modal", host.Markup);
        Assert.False(host.Instance.Modal!.IsVisible);
        Assert.Equal(1, host.Instance.OnCloseCallCount);
    }

    [Fact]
    public void Modal_CloseViaOverlayClick()
    {
        // Arrange
        var host = RenderComponent<ModalHost>();
        host.Find("#open-modal-button").Click();

        // Act - Click on overlay
        var overlay = host.Find(".bcl-modal-overlay");
        overlay.Click();

        // Assert
        Assert.DoesNotContain(".bcl-modal-overlay", host.Markup);
        Assert.DoesNotContain(".bcl-modal", host.Markup);
        Assert.False(host.Instance.Modal!.IsVisible);
        Assert.Equal(1, host.Instance.OnCloseCallCount);
    }

    [Fact]
    public void Modal_CloseViaEscapeKey()
    {
        // Arrange
        var host = RenderComponent<ModalHost>();
        host.Find("#open-modal-button").Click();

        // Act - Press Escape key while modal is open
        var dialog = host.Find(".bcl-modal");
        dialog.KeyDown("Escape");

        // Assert
        Assert.DoesNotContain(".bcl-modal-overlay", host.Markup);
        Assert.DoesNotContain(".bcl-modal", host.Markup);
        Assert.False(host.Instance.Modal!.IsVisible);
        Assert.Equal(1, host.Instance.OnCloseCallCount);
    }

    [Fact]
    public void Modal_CloseCallbacksFireExactlyOnceEach()
    {
        // Arrange
        var host = RenderComponent<ModalHost>();
        host.Find("#open-modal-button").Click();

        // Act - Close via close button
        host.Find(".bcl-modal-close").Click();

        // Assert
        Assert.Equal(1, host.Instance.OnCloseCallCount);
        Assert.False(host.Instance.Modal!.IsVisible);

        // Act - Open again and close via overlay
        host.Find("#open-modal-button").Click();
        host.Find(".bcl-modal-overlay").Click();

        // Assert
        Assert.Equal(2, host.Instance.OnCloseCallCount);
        Assert.False(host.Instance.Modal!.IsVisible);
    }

    [Fact]
    public void Modal_DOMCleanedUpAfterClose()
    {
        // Arrange
        var host = RenderComponent<ModalHost>();
        host.Find("#open-modal-button").Click();

        // Assert - Modal elements present
        Assert.NotNull(host.Find(".bcl-modal-overlay"));
        Assert.NotNull(host.Find(".bcl-modal"));

        // Act - Close modal
        host.Find(".bcl-modal-close").Click();

        // Assert - Modal elements removed from DOM
        Assert.Throws<Bunit.ElementNotFoundException>(() => host.Find(".bcl-modal-overlay"));
        Assert.Throws<Bunit.ElementNotFoundException>(() => host.Find(".bcl-modal"));
    }

    // Note: Focus trapping verification is skipped because the Modal component does not appear to implement
    // explicit focus trapping logic (only initial focus on dialog and restore on close).
    // If focus trapping is added in the future, this test can be implemented.
}