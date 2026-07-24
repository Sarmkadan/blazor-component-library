namespace BlazorComponentLibrary.Components.Skeleton;

using Microsoft.AspNetCore.Components;

public sealed partial class Skeleton : ComponentBase, ISkeleton
{
	[Parameter]
	public SkeletonType Type { get; set; } = SkeletonType.Text;

	[Parameter]
	public SkeletonShape Shape { get; set; } = SkeletonShape.Rect;

	[Parameter]
	public string Width { get; set; } = "100%";

	[Parameter]
	public string Height { get; set; } = "auto";

	/// <summary>
	/// Number of text lines rendered when <see cref="Type"/> is <see cref="SkeletonType.Text"/>.
	/// Ignored for Circle and Rectangle types.
	/// </summary>
	[Parameter]
	public int Lines { get; set; } = 3;

	/// <summary>
	/// When true, a CSS pulse animation is applied to the placeholder to indicate
	/// that content is loading.
	/// </summary>
	[Parameter]
	public bool Animated { get; set; } = true;

	/// <summary>
	/// Optional label text to describe the loading state for screen readers.
	/// Defaults to "Loading…".
	/// </summary>
	[Parameter]
	public string? LoadingLabel { get; set; }

	/// <summary>
	/// When true, renders a visually-hidden label for the loading state.
	/// </summary>
	[Parameter]
	public bool ShowLoadingLabel { get; set; }

	private string CssClass => Animated ? "bcl-skeleton bcl-skeleton--animated" : "bcl-skeleton";

	private string GetAriaBusyAttribute()
	{
		return "aria-busy=\"true\"";
	}

	private string GetLoadingLabel()
	{
		return LoadingLabel ?? "Loading…";
	}

	private string ShapeClass
	{
		get
		{
			return Shape switch
			{
				SkeletonShape.Text => "bcl-skeleton--text",
				SkeletonShape.Circle => "bcl-skeleton--circle",
				SkeletonShape.Rect => "bcl-skeleton--rectangle",
				SkeletonShape.Card => "bcl-skeleton--card",
				_ => "bcl-skeleton--rectangle"
			};
		}
	}
}