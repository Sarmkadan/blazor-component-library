namespace BlazorComponentLibrary.Components.ThemeSwitcher;

using System;
using System.Threading.Tasks;
using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

/// <summary>
/// A three‑way toggle that lets users switch between Light, Dark, and System themes.
/// The selected preference is persisted via <see cref="IThemeService"/> so the choice
/// survives page reloads. Subscribe to <see cref="IThemeService.ThemeChanged"/> to
/// react to theme changes elsewhere in the application.
/// </summary>
public sealed partial class ThemeSwitcher : ComponentBase, IThemeSwitcher, IDisposable, IAsyncDisposable
{
    [Inject]
    private IThemeService ThemeService { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    /// <inheritdoc/>
    [Parameter]
    public bool ShowLabel { get; set; } = true;

    /// <inheritdoc/>
    [Parameter]
    public string? CssClass { get; set; }

    private string RootClass =>
        string.IsNullOrEmpty(CssClass)
            ? "bcl-theme-switcher"
            : $"bcl-theme-switcher {CssClass}";

    /// <summary>
    /// Subscribes to the <see cref="IThemeService.ThemeChanged"/> event.
    /// </summary>
    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(ThemeService);
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    /// <summary>
    /// Performs JavaScript interop after the component is rendered for the first time.
    /// Reads any persisted theme from <c>localStorage</c> and, if none is found,
    /// falls back to the browser's <c>prefers-color-scheme</c> media query.
    /// This method is safe for prerendering scenarios – any JS interop failures are
    /// caught and ignored, allowing the component to render without a theme until
    /// the client side becomes available.
    /// </summary>
    /// <param name="firstRender">True if this is the first render.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadAndApplyThemeAsync();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Loads a persisted theme from <c>localStorage</c> (if present) and applies it.
    /// If no persisted value exists, falls back to the browser's
    /// <c>prefers-color-scheme</c> media query. All JavaScript interop calls are
    /// wrapped in <c>try/catch</c> blocks to avoid exceptions during prerendering.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    private async Task LoadAndApplyThemeAsync()
    {
        // Try to read a persisted theme from localStorage.
        try
        {
            var stored = await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "bcl-theme");
            if (!string.IsNullOrWhiteSpace(stored) &&
                Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var parsed))
            {
                ThemeService.SetTheme(parsed);
                return;
            }
        }
        catch (JSException)
        {
            // JS interop not available (e.g., prerender). Continue to fallback.
        }

        // No stored value – fall back to prefers-color-scheme.
        try
        {
            var prefersDark = await JSRuntime.InvokeAsync<bool>(
                "eval",
                "window.matchMedia('(prefers-color-scheme: dark)').matches");

            var fallback = prefersDark ? ThemeMode.Dark : ThemeMode.Light;
            ThemeService.SetTheme(fallback);
        }
        catch (JSException)
        {
            // If even this fails, default to Light.
            ThemeService.SetTheme(ThemeMode.Light);
        }
    }

    private void OnThemeChanged(ThemeMode _) => InvokeAsync(StateHasChanged);

    /// <summary>
    /// Selects the specified <paramref name="mode"/> and persists the choice in
    /// <c>localStorage</c>. Any JavaScript interop failures are ignored to keep the
    /// component functional during prerendering.
    /// </summary>
    /// <param name="mode">The theme mode to apply.</param>
    private void Select(ThemeMode mode)
    {
        ThemeService.SetTheme(mode);
        try
        {
            _ = JSRuntime.InvokeVoidAsync("localStorage.setItem", "bcl-theme", mode.ToString());
        }
        catch (JSException)
        {
            // Ignore failures (e.g., prerendering).
        }
    }

    internal bool IsActive(ThemeMode mode) => ThemeService.CurrentTheme == mode;

    /// <inheritdoc/>
    public void Dispose()
    {
        ArgumentNullException.ThrowIfNull(ThemeService);
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
