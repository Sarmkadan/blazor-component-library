namespace BlazorComponentLibrary.Components.Modal;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public static class ModalValidation
{
    /// <summary>
    /// Validates the given <paramref name="value"/> and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The <see cref="Modal"/> to validate.</param>
    /// <returns>A list of human-readable problems.</returns>
    public static IReadOnlyList<string> Validate(this Modal value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        if (string.IsNullOrEmpty(value.Title))
        {
            problems.Add("Title is required.");
        }

        if (value.CloseOnOverlayClick && value.OnClose.HasDelegate)
        {
            problems.Add("Cannot set CloseOnOverlayClick to true when OnClose is set.");
        }

        return problems;
    }

    /// <summary>
    /// Checks if the given <paramref name="value"/> is valid.
    /// </summary>
    /// <param name="value">The <see cref="Modal"/> to check.</param>
    /// <returns>True if the <paramref name="value"/> is valid, false otherwise.</returns>
    public static bool IsValid(this Modal value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return !Validate(value).Any();
    }

    /// <summary>
    /// Ensures that the given <paramref name="value"/> is valid.
    /// </summary>
    /// <param name="value">The <see cref="Modal"/> to ensure is valid.</param>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="value"/> is invalid.</exception>
    public static void EnsureValid(this Modal value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = Validate(value);

        if (problems.Any())
        {
            throw new ArgumentException($"The following problems were found: {string.Join(", ", problems)}", nameof(value));
        }
    }
}
