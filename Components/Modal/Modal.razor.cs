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

public sealed partial class Modal : ComponentBase, IModal, IDisposable, IAsyncDisposable
{
    private bool _isVisible = false;
    private ElementReference _dialogElement;
    private ElementReference? _triggerElement;
    private bool _disposed;

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
        // Save the currently focused element before showing the modal
        _triggerElement = await GetActiveElementReference();

        _isVisible = true;
        StateHasChanged();

        // Focus the dialog after it renders
        await Task.Delay(100); // Allow time for the dialog to render
        try
        {
            await _dialogElement.FocusAsync();
        }
        catch
        {
            // Ignore focus errors (element might not be ready)
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

        // Restore focus to the trigger element
        if (_triggerElement.HasValue)
        {
            try
            {
                await _triggerElement.Value.FocusAsync();
            }
            catch
            {
                // Ignore focus errors (element might be disposed)
            }
        }
    }

    /// <summary>
    /// Gets a reference to the currently focused element.
    /// </summary>
    /// <returns>ElementReference to the active element, or default if none.</returns>
    private async Task<ElementReference> GetActiveElementReference()
    {
        try
        {
            return await JSRuntime.InvokeAsync<ElementReference>("getActiveElement");
        }
        catch
        {
            // Fallback to default if JS interop fails
            return default;
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

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        // Clean up any JavaScript interop resources
        _disposed = true;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        // Clean up any JavaScript interop resources
        _disposed = true;
        await ValueTask.CompletedTask;
    }
}