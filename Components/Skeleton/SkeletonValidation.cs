namespace BlazorComponentLibrary.Components.Skeleton;

/// <summary>
/// Provides validation helpers for <see cref="Skeleton"/> instances.
/// </summary>
public static class SkeletonValidation
{
    /// <summary>
    /// Validates a <see cref="Skeleton"/> instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The skeleton instance to validate.</param>
    /// <returns>An enumerable of validation messages. Empty if valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
    public static IReadOnlyList<string> Validate(this Skeleton value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = new List<string>();

        // Validate Type
        if (value.Type == default)
        {
            errors.Add("Skeleton.Type must be set to Text, Circle, or Rectangle.");
        }

        // Validate Width
        if (string.IsNullOrWhiteSpace(value.Width))
        {
            errors.Add("Skeleton.Width must be a non-null, non-empty CSS value.");
        }

        // Validate Height
        if (string.IsNullOrWhiteSpace(value.Height))
        {
            errors.Add("Skeleton.Height must be a non-null, non-empty CSS value.");
        }

        // Validate Lines (only relevant for Text type)
        if (value.Type == SkeletonType.Text && value.Lines < 1)
        {
            errors.Add("Skeleton.Lines must be greater than 0 when Type is Text.");
        }

        // Validate Animated
        // No validation needed for boolean - always valid

        return errors.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="Skeleton"/> instance is valid.
    /// </summary>
    /// <param name="value">The skeleton instance to check.</param>
    /// <returns><see langword="true"/> if valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
    public static bool IsValid(this Skeleton value)
    {
        return value.Validate().Count == 0;
    }

    /// <summary>
    /// Ensures that a <see cref="Skeleton"/> instance is valid, throwing an <see cref="ArgumentException"/> if not.
    /// </summary>
    /// <param name="value">The skeleton instance to validate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is invalid. The exception message contains the validation errors.</exception>
    public static void EnsureValid(this Skeleton value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = value.Validate();

        if (errors.Count > 0)
        {
            throw new ArgumentException(
                $"Skeleton validation failed:{Environment.NewLine}- {
                    string.Join($"{Environment.NewLine}- ", errors)}");
        }
    }
}