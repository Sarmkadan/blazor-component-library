namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
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
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="jsRuntime"/> is null.</exception>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <inheritdoc/>
    /// <exception cref="ThemeServiceException">Thrown when there is an error accessing local storage or applying theme.</exception>
    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _jsRuntime.InvokeAsync<string?>(
                "eval", "localStorage.getItem('bcl-theme')")
                .ConfigureAwait(false);

            if (Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var persisted))
                ApplyTheme(persisted, persist: false);
        }
        catch (Exception ex) when (ex is not ThemeServiceException)
        {
            // JS interop is unavailable during server-side pre-rendering; silently ignore.
            // Other exceptions are wrapped and rethrown as ThemeServiceException
            throw new ThemeServiceException("Failed to initialize theme service", ex);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="ThemeServiceException">Thrown when there is an error setting or persisting the theme.</exception>
    public void SetTheme(ThemeMode theme) => ApplyTheme(theme, persist: true);

    private void ApplyTheme(ThemeMode theme, bool persist)
    {
        _currentTheme = theme;

        var attributeValue = theme switch
        {
            ThemeMode.Dark => "dark",
            ThemeMode.Light => "light",
            _ => "auto",
        };

        try
        {
            _ = _jsRuntime.InvokeVoidAsync(
                "eval",
                $"document.documentElement.setAttribute('data-bcl-theme', '{attributeValue}')")
                .ConfigureAwait(false);

            if (persist)
                _ = _jsRuntime.InvokeVoidAsync(
                    "eval",
                    $"localStorage.setItem('bcl-theme', '{theme}')")
                    .ConfigureAwait(false);

            ThemeChanged?.Invoke(theme);
        }
        catch (Exception ex)
        {
            throw new ThemeServiceException("Failed to apply theme", ex);
        }
    }
}
