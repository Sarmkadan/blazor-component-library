namespace BlazorComponentLibrary.Components.Modal;

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public interface IModal
{
    /// <summary>
    /// Shows the modal dialog.
    /// </summary>
    /// <returns>A task that represents the asynchronous show operation.</returns>
    Task Show();

    /// <summary>
    /// Hides the modal dialog.
    /// </summary>
    /// <returns>A task that represents the asynchronous hide operation.</returns>
    Task Hide();

    /// <summary>
    /// Gets a value indicating whether the modal is currently visible.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Gets or sets the title of the modal.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the content for the modal's body.
    /// </summary>
    RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the content for the modal's footer.
    /// </summary>
    RenderFragment FooterContent { get; set; }

    /// <summary>
    /// Event callback for when the modal is closed.
    /// </summary>
    EventCallback OnClose { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether clicking the overlay should close the modal.
    /// </summary>
    bool CloseOnOverlayClick { get; set; }
}
