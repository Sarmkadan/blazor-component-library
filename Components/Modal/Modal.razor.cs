namespace BlazorComponentLibrary.Components.Modal;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public enum ModalSize
{
    Small,
    Medium,
    Large,
    FullScreen
}

public sealed partial class Modal : ComponentBase, IModal
{
    private bool _isVisible = false;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? FooterContent { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public bool CloseOnOverlayClick { get; set; } = true;

    [Parameter]
    public ModalSize Size { get; set; } = ModalSize.Medium;

    /// <summary>
    /// Gets a value indicating whether the modal is currently visible.
    /// </summary>
    public bool IsVisible => _isVisible;

    /// <summary>
    /// Gets the CSS class string corresponding to the current <see cref="Size"/>.
    /// </summary>
    public string SizeClass => Size switch
    {
        ModalSize.Small => "modal-small",
        ModalSize.Medium => "modal-medium",
        ModalSize.Large => "modal-large",
        ModalSize.FullScreen => "modal-fullscreen",
        _ => "modal-medium"
    };

    /// <summary>
    /// Shows the modal dialog. Saves the currently focused element so it can be
    /// restored when the modal closes (WCAG 2.1 SC 2.4.3).
    /// </summary>
    /// <exception cref="ModalException">Thrown when there is an error showing the modal.</exception>
    public async Task Show()
    {
        if (JSRuntime is null)
        {
            throw new ModalException("JavaScript runtime is not available.");
        }

        try
        {
            // No ConfigureAwait(false) here: StateHasChanged must run on the
            // component's dispatcher (sync context), otherwise Blazor Server throws.
            await JSRuntime.InvokeVoidAsync("eval", "window.__bclModalTrigger = document.activeElement");
            _isVisible = true;
            StateHasChanged();
        }
        catch (Exception ex) when (ex is not ModalException)
        {
            throw new ModalException("Failed to show modal", ex);
        }
    }

    /// <summary>
    /// Hides the modal dialog and restores focus to the element that opened it.
    /// </summary>
    /// <exception cref="ModalException">Thrown when there is an error hiding the modal.</exception>
    public async Task Hide()
    {
        _isVisible = false;
        StateHasChanged();
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync().ConfigureAwait(false);
        }

        if (JSRuntime is not null)
        {
            try
            {
                await JSRuntime.InvokeVoidAsync(
                    "eval",
                    "if (window.__bclModalTrigger && typeof window.__bclModalTrigger.focus === 'function') { window.__bclModalTrigger.focus(); window.__bclModalTrigger = null; }"
                ).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not ModalException)
            {
                throw new ModalException("Failed to restore focus after hiding modal", ex);
            }
        }
    }

    protected Task HandleOverlayClick()
    {
        if (CloseOnOverlayClick)
        {
            return Hide();
        }

        return Task.CompletedTask;
    }
}
