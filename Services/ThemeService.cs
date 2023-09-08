namespace BlazorComponentLibrary.Services;

using Microsoft.JSInterop;

/// <summary>
/// Default implementation of <see cref="IThemeService"/>.
/// Writes a <c>data-bcl-theme</c> attribute (<c>"light"</c>, <c>"dark"</c>, or <c>"auto"</c>)
/// to <c>document.documentElement</c> so CSS custom properties can vary by theme without
/// a full page reload. The selected preference is persisted in <c>localStorage</c> under
/// the key <c>bcl-theme</c>.
/// </summary>
public sealed class ThemeService : IThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private ThemeMode _currentTheme = ThemeMode.System;

    /// <inheritdoc/>
    public ThemeMode CurrentTheme => _currentTheme;

    /// <inheritdoc/>
    public event Action<ThemeMode>? ThemeChanged;

    /// <summary>Initialises a new instance of <see cref="ThemeService"/>.</summary>
    /// <param name="jsRuntime">The JS interop runtime injected by the DI container.</param>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _jsRuntime.InvokeAsync<string?>(
                "eval", "localStorage.getItem('bcl-theme')");

            if (Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var persisted))
                ApplyTheme(persisted, persist: false);
        }
        catch
        {
            // JS interop is unavailable during server-side pre-rendering; silently ignore.
        }
    }

    /// <inheritdoc/>
    public void SetTheme(ThemeMode theme) => ApplyTheme(theme, persist: true);

    private void ApplyTheme(ThemeMode theme, bool persist)
    {
        _currentTheme = theme;

        var attributeValue = theme switch
        {
            ThemeMode.Dark  => "dark",
            ThemeMode.Light => "light",
            _               => "auto",
        };

        _ = _jsRuntime.InvokeVoidAsync(
            "eval",
            $"document.documentElement.setAttribute('data-bcl-theme', '{attributeValue}')");

        if (persist)
            _ = _jsRuntime.InvokeVoidAsync(
                "eval",
                $"localStorage.setItem('bcl-theme', '{theme}')");

        ThemeChanged?.Invoke(theme);
    }
}
