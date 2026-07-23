namespace BlazorComponentLibrary.Components.Toast;

using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Components;
using System;

/// <summary>
/// Renders the stack of active toast notifications produced by <see cref="IToastService"/>.
/// Place a single <c>&lt;ToastContainer /&gt;</c> in your root layout component so it
/// is always present regardless of the current route.
/// </summary>
public sealed partial class ToastContainer : ComponentBase, IToastContainer, IDisposable, IAsyncDisposable
{
    [Inject]
    internal IToastService ToastService { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public ToastPosition Position { get; set; } = ToastPosition.BottomRight;

    /// <inheritdoc/>
    [Parameter]
    public int MaxVisible { get; set; } = 5;

    internal IEnumerable<ToastMessage> VisibleToasts =>
        ToastService.ActiveToasts.TakeLast(MaxVisible);

    private string ContainerClass =>
        $"bcl-toast-container bcl-toast-container--{PositionCss(Position)}";

    protected override void OnInitialized()
    {
        ToastService.ToastsChanged += OnToastsChanged;
    }

    private void OnToastsChanged() => InvokeAsync(StateHasChanged);

    /// <summary>Returns the icon character for the given toast type.</summary>
    public static string IconFor(ToastType type) => type switch
    {
        ToastType.Success => "✓",
        ToastType.Warning => "⚠",
        ToastType.Error => "✕",
        _ => "ℹ",
    };

    /// <summary>Gets the icon to display for the given toast message.</summary>
    /// <param name="toast">The toast message.</param>
    /// <returns>The icon to display, or null if no icon should be displayed.</returns>
    public static string? GetToastIcon(ToastMessage toast) => toast.Icon ?? IconFor(toast.Type);

    private static string PositionCss(ToastPosition position) => position switch
    {
        ToastPosition.TopLeft => "top-left",
        ToastPosition.TopCenter => "top-center",
        ToastPosition.TopRight => "top-right",
        ToastPosition.BottomLeft => "bottom-left",
        ToastPosition.BottomCenter => "bottom-center",
        _ => "bottom-right",
    };

    /// <inheritdoc/>
    public void Dispose()
    {
        ToastService.ToastsChanged -= OnToastsChanged;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
