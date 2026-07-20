namespace BlazorComponentLibrary.Components.Skeleton;

public enum SkeletonType
{
Text,
Circle,
Rectangle
}

public enum SkeletonShape
{
Text,
Circle,
Rect,
Card
}

public interface ISkeleton
{
/// <summary>Gets or sets the shape type of the skeleton placeholder.</summary>
SkeletonType Type { get; set; }

/// <summary>Gets or sets the shape of the skeleton.</summary>
SkeletonShape Shape { get; set; }

/// <summary>Gets or sets the CSS width of the skeleton (e.g. "100%", "200px").</summary>
string Width { get; set; }

/// <summary>Gets or sets the CSS height of the skeleton (e.g. "auto", "80px").</summary>
string Height { get; set; }

/// <summary>Gets or sets the number of text lines rendered when Type is Text.</summary>
int Lines { get; set; }

/// <summary>Gets or sets whether the pulse animation is enabled.</summary>
bool Animated { get; set; }
}