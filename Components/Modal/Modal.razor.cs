namespace BlazorComponentLibrary.Components.Modal;

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public partial class Modal : ComponentBase, IModal
{
    private bool _isVisible = false;

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
    /// Shows the modal dialog.
    /// </summary>
    public Task Show()
    {
        _isVisible = true;
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Hides the modal dialog.
    /// </summary>
    public async Task Hide()
    {
        _isVisible = false;
        StateHasChanged();
        if (OnClose.HasDelegate)
        {
            await OnClose.InvokeAsync();
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
