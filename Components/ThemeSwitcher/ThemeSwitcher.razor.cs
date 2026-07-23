namespace BlazorComponentLibrary.Components.ThemeSwitcher;

using BlazorComponentLibrary.Services;
using Microsoft.AspNetCore.Components;
using System;

/// <summary>
/// A three-way toggle that lets users switch between Light, Dark, and System themes.
/// The selected preference is persisted via <see cref="IThemeService"/> so the choice
/// survives page reloads. Subscribe to <see cref="IThemeService.ThemeChanged"/> to
/// react to theme changes elsewhere in the application.
/// </summary>
public sealed partial class ThemeSwitcher : ComponentBase, IThemeSwitcher, IDisposable, IAsyncDisposable
{
    [Inject]
    private IThemeService ThemeService { get; set; } = default!;

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

    protected override void OnInitialized()
    {
        ThemeService.ThemeChanged += OnThemeChanged;
    }

    private void OnThemeChanged(ThemeMode _) => InvokeAsync(StateHasChanged);

    private void Select(ThemeMode mode) => ThemeService.SetTheme(mode);

    internal bool IsActive(ThemeMode mode) => ThemeService.CurrentTheme == mode;

    /// <inheritdoc/>
    public void Dispose()
    {
        ThemeService.ThemeChanged -= OnThemeChanged;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
