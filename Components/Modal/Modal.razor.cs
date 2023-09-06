namespace BlazorComponentLibrary.Components.Modal;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public partial class Modal : ComponentBase, IModal
{
    private bool _isVisible = false;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment FooterContent { get; set; }

    [Parameter]
    public EventCallback OnClose { get; set; }

    [Parameter]
    public bool CloseOnOverlayClick { get; set; } = true;

    /// <summary>
    /// Gets a value indicating whether the modal is currently visible.
    /// </summary>
    public bool IsVisible => _isVisible;

    /// <summary>
    /// Shows the modal dialog. Saves the currently focused element so it can be
    /// restored when the modal closes (WCAG 2.1 SC 2.4.3).
    /// </summary>
    public async Task Show()
    {
        await JSRuntime.InvokeVoidAsync("eval", "window.__bclModalTrigger = document.activeElement");
        _isVisible = true;
        StateHasChanged();
    }

    /// <summary>
    /// Hides the modal dialog and restores focus to the element that opened it.
    /// </summary>
    public async Task Hide()
    {
        _isVisible = false;
        StateHasChanged();
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
        }
        await JSRuntime.InvokeVoidAsync(
            "eval",
            "if (window.__bclModalTrigger && typeof window.__bclModalTrigger.focus === 'function') { window.__bclModalTrigger.focus(); window.__bclModalTrigger = null; }"
        );
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
