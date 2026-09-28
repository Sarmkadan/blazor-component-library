using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorComponentLibrary.Tests.Components.Accordion
{
    /// <summary>
    /// Extension methods for <see cref="AccordionItem"/> to facilitate testing.
    /// </summary>
    public static class AccordionItemExtensions
    {
        /// <summary>
        /// Toggles the expanded state of the accordion item.
        /// </summary>
        /// <param name="item">The accordion item to toggle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="item"/> is <see langword="null"/>.</exception>
        public static void ToggleExpanded(this AccordionItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.SetExpanded(!item.IsExpanded);
        }

        /// <summary>
        /// Gets the CSS class that would be applied to the accordion item based on its expanded state.
        /// </summary>
        /// <param name="item">The accordion item to evaluate.</param>
        /// <returns>"expanded" if the item is expanded; otherwise, an empty string.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="item"/> is <see langword="null"/>.</exception>
        public static string GetDisplayClass(this AccordionItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return item.IsExpanded ? "expanded" : string.Empty;
        }

        /// <summary>
        /// Invokes the <see cref="AccordionItem.OnClick"/> callback if it has a delegate assigned.
        /// </summary>
        /// <param name="item">The accordion item whose click callback to invoke.</param>
        /// <param name="eventArgs">
        /// Optional mouse event arguments to pass to the callback. If <see langword="null"/>, a default <see cref="MouseEventArgs"/> is used.
        /// </param>
        /// <returns>A task that completes when the callback has been invoked.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="item"/> is <see langword="null"/>.</exception>
        public static async Task ClickAsync(this AccordionItem item, MouseEventArgs? eventArgs = null)
        {
            ArgumentNullException.ThrowIfNull(item);
            if (item.OnClick.HasDelegate)
            {
                await item.OnClick.InvokeAsync(eventArgs ?? new MouseEventArgs());
            }
        }
    }
}