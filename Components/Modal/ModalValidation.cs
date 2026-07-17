namespace BlazorComponentLibrary.Components.Modal;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ModalValidation
{
    /// <summary>
    /// Validates the given <paramref name="modal"/> and returns a list of human-readable problems.
    /// </summary>
    /// <param name="modal">The <see cref="Modal"/> to validate.</param>
    /// <returns>A list of human-readable problems.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);

        var problems = new List<string>();

        if (string.IsNullOrWhiteSpace(modal.Title))
        {
            problems.Add("Title is required.");
        }

        if (modal.CloseOnOverlayClick && modal.OnClose.HasDelegate)
        {
            problems.Add("Cannot set CloseOnOverlayClick to true when OnClose is set.");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Checks if the given <paramref name="modal"/> is valid.
    /// </summary>
    /// <param name="modal">The <see cref="Modal"/> to check.</param>
    /// <returns>True if the <paramref name="modal"/> is valid, false otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modal"/> is <see langword="null"/>.</exception>
    public static bool IsValid(this Modal modal) => modal is not null && !Validate(modal).Any();

    /// <summary>
    /// Ensures that the given <paramref name="modal"/> is valid.
    /// </summary>
    /// <param name="modal">The <see cref="Modal"/> to ensure is valid.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="modal"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="modal"/> is invalid.</exception>
    public static void EnsureValid(this Modal modal)
    {
        ArgumentNullException.ThrowIfNull(modal);

        var problems = Validate(modal);

        if (problems.Count > 0)
        {
            throw new ArgumentException($"The following problems were found: {string.Join(", ", problems)}", nameof(modal));
        }
    }
}
