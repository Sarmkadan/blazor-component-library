namespace BlazorComponentLibrary.Components.Toast;

using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;

/// <summary>
/// Renders the stack of active toast notifications produced by <see cref="IToastService"/>.
/// Place a single <c>&lt;ToastContainer /&gt;</c> in your root layout component so it is
/// always present regardless of the current route.
/// </summary>
public sealed partial class ToastContainer : ComponentBase, IToastContainer, IDisposable, IAsyncDisposable
{
    private ToastPosition? _position;
    private int? _maxVisible;
    private bool? _pauseOnHover;

    [Inject]
    internal IToastService ToastService { get; set; } = default!;

    [Inject]
    internal IOptions<ToastOptions> Options { get; set; } = default!;

    /// <inheritdoc/>
    /// <remarks>When not supplied, falls back to <see cref="ToastOptions.Position"/>.</remarks>
    [Parameter]
    public ToastPosition Position
    {
        get => _position ?? Options.Value.Position;
        set => _position = value;
    }

    /// <inheritdoc/>
    /// <remarks>When not supplied, falls back to <see cref="ToastOptions.MaxVisibleToasts"/>.</remarks>
    [Parameter]
    public int MaxVisible
    {
        get => _maxVisible ?? Options.Value.MaxVisibleToasts;
        set => _maxVisible = value;
    }

    /// <inheritdoc/>
    /// <remarks>When not supplied, falls back to <see cref="ToastOptions.PauseOnHover"/>.</remarks>
    [Parameter]
    public bool PauseOnHover
    {
        get => _pauseOnHover ?? Options.Value.PauseOnHover;
        set => _pauseOnHover = value;
    }

    /// <inheritdoc/>
    [Parameter]
    public bool Dedup { get; set; } = false;

    internal IEnumerable<ToastMessage> VisibleToasts =>
        ToastService.ActiveToasts.TakeLast(MaxVisible);

    private string ContainerClass =>
        $"bcl-toast-container bcl-toast-container--{PositionCss(Position)}";

    protected override void OnInitialized()
    {
        ToastService.ToastsChanged += OnToastsChanged;
    }

    private void OnToastsChanged() => InvokeAsync(StateHasChanged);

    // Hover handlers are always bound and check PauseOnHover here, because a conditional
    // attribute inside the element's start tag is not emitted as Razor control flow.
    private void PauseOnHoverStart(Guid toastId)
    {
        if (PauseOnHover)
        {
            ToastService.PauseTimer(toastId);
        }
    }

    private void PauseOnHoverEnd(Guid toastId, int durationMs)
    {
        if (PauseOnHover)
        {
            ToastService.ResumeTimer(toastId, durationMs);
        }
    }

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
