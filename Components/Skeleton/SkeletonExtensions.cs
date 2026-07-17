namespace BlazorComponentLibrary.Components.Skeleton;

/// <summary>
/// Provides extension methods for <see cref="Skeleton"/> components to enable fluent configuration.
/// </summary>
public static class SkeletonExtensions
{
    /// <summary>
    /// Sets the skeleton type to Text with the specified number of lines.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="lines">Number of text lines to render. Must be greater than 0.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="lines"/> is less than 1.</exception>
    public static Skeleton AsText(this Skeleton skeleton, int lines = 3)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentOutOfRangeException.ThrowIfLessThan(lines, 1);

        skeleton.Type = SkeletonType.Text;
        skeleton.Lines = lines;
        return skeleton;
    }

    /// <summary>
    /// Sets the skeleton type to Circle with the specified size.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="size">The width and height of the circle (e.g. "40px", "2rem").</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="size"/> is <see langword="null"/> or whitespace.</exception>
    public static Skeleton AsCircle(this Skeleton skeleton, string size = "40px")
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentException.ThrowIfNullOrWhiteSpace(size);

        skeleton.Type = SkeletonType.Circle;
        skeleton.Width = size;
        skeleton.Height = size;
        return skeleton;
    }

    /// <summary>
    /// Sets the skeleton type to Rectangle with the specified dimensions.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="width">The width of the rectangle (e.g. "100%", "200px").</param>
    /// <param name="height">The height of the rectangle (e.g. "80px", "10rem").</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="width"/> is <see langword="null"/> or whitespace.</exception>
    public static Skeleton AsRectangle(this Skeleton skeleton, string width = "100%", string height = "auto")
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentException.ThrowIfNullOrWhiteSpace(width);

        skeleton.Type = SkeletonType.Rectangle;
        skeleton.Width = width;
        skeleton.Height = height ?? "auto";
        return skeleton;
    }

    /// <summary>
    /// Sets the width of the skeleton.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="width">The CSS width value (e.g. "100%", "200px", "5rem").</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="width"/> is <see langword="null"/> or whitespace.</exception>
    public static Skeleton WithWidth(this Skeleton skeleton, string width)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentException.ThrowIfNullOrWhiteSpace(width);

        skeleton.Width = width;
        return skeleton;
    }

    /// <summary>
    /// Sets the height of the skeleton.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="height">The CSS height value (e.g. "auto", "80px", "10rem").</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentException"><paramref name="height"/> is <see langword="null"/> or whitespace.</exception>
    public static Skeleton WithHeight(this Skeleton skeleton, string height)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentException.ThrowIfNullOrWhiteSpace(height);

        skeleton.Height = height;
        return skeleton;
    }

    /// <summary>
    /// Sets the number of lines for text skeleton types.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="lines">Number of text lines to render. Must be greater than 0.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="lines"/> is less than 1.</exception>
    public static Skeleton WithLines(this Skeleton skeleton, int lines)
    {
        ArgumentNullException.ThrowIfNull(skeleton);
        ArgumentOutOfRangeException.ThrowIfLessThan(lines, 1);

        skeleton.Lines = lines;
        return skeleton;
    }

    /// <summary>
    /// Enables or disables the animation effect on the skeleton.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <param name="animated">Whether to enable the pulse animation.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton WithAnimation(this Skeleton skeleton, bool animated)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        skeleton.Animated = animated;
        return skeleton;
    }

    /// <summary>
    /// Sets the skeleton to be animated.
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton Animated(this Skeleton skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        skeleton.Animated = true;
        return skeleton;
    }

    /// <summary>
    /// Sets the skeleton to be static (no animation).
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton Static(this Skeleton skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        skeleton.Animated = false;
        return skeleton;
    }

    /// <summary>
    /// Configures the skeleton with common avatar dimensions (circle with 40px size).
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton AsAvatar(this Skeleton skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        return skeleton.AsCircle("40px");
    }

    /// <summary>
    /// Configures the skeleton with common button dimensions (rectangle with 120px width and 40px height).
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton AsButton(this Skeleton skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        return skeleton.AsRectangle("120px", "40px");
    }

    /// <summary>
    /// Configures the skeleton with common card dimensions (rectangle with 100% width and 200px height).
    /// </summary>
    /// <param name="skeleton">The skeleton instance to configure.</param>
    /// <returns>The configured skeleton instance for method chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="skeleton"/> is <see langword="null"/></exception>
    public static Skeleton AsCard(this Skeleton skeleton)
    {
        ArgumentNullException.ThrowIfNull(skeleton);

        return skeleton.AsRectangle("100%", "200px");
    }
}
