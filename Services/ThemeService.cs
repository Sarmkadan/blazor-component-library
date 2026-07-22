namespace BlazorComponentLibrary.Services;

using BlazorComponentLibrary.Exceptions;
using Microsoft.AspNetCore.Components;
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
    private readonly object _eventLock = new();

    /// <inheritdoc/>
    public ThemeMode CurrentTheme => _currentTheme;

    /// <inheritdoc/>
    public event Action<ThemeMode>? ThemeChanged;

    /// <summary>
    /// Raised when the theme changes, providing the string representation
    /// that is written to the <c>data-bcl-theme</c> attribute (e.g., "light", "dark", "auto").
    /// </summary>
    public event Action<string>? OnThemeChanged;

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
        string? stored;
        try
        {
            stored = await _jsRuntime.InvokeAsync<string?>(
                "eval", "localStorage.getItem('bcl-theme')")
                .ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not ThemeServiceException)
        {
            throw new ThemeServiceException("Failed to initialize theme service", ex);
        }

        // Enum.IsDefined guards against stray numeric strings (e.g. "42") which
        // Enum.TryParse would otherwise happily convert to an undefined enum value.
        if (Enum.TryParse<ThemeMode>(stored, ignoreCase: true, out var persisted) &&
            Enum.IsDefined(typeof(ThemeMode), persisted))
        {
            ApplyTheme(persisted, persist: false);
        }
        // If the stored value is null, empty, or not a defined ThemeMode,
        // we simply keep the default (ThemeMode.System) without throwing.
    }

    /// <inheritdoc/>
    public void SetTheme(ThemeMode theme) => ApplyTheme(theme, persist: true);

    private void ApplyTheme(ThemeMode theme, bool persist)
    {
        // Do not raise events or perform any work if the theme is unchanged.
        if (theme == _currentTheme)
        {
            return;
        }

        _currentTheme = theme;

        var attributeValue = theme switch
        {
            ThemeMode.Dark => "dark",
            ThemeMode.Light => "light",
            _ => "auto",
        };

        // The interop calls are intentionally fire-and-forget: SetTheme is synchronous
        // by contract, and the in-memory theme remains authoritative even if the browser
        // update fails. Faults are observed inside PushToBrowserAsync so they never
        // surface as unobserved task exceptions.
        _ = PushToBrowserAsync(attributeValue, persist ? theme : null);

        InvokeThemeChanged(theme);
        InvokeOnThemeChanged(attributeValue);
    }

    private async Task PushToBrowserAsync(string attributeValue, ThemeMode? persistAs)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync(
                "eval",
                $"document.documentElement.setAttribute('data-bcl-theme', '{attributeValue}')")
                .ConfigureAwait(false);

            if (persistAs is { } theme)
                await _jsRuntime.InvokeVoidAsync(
                    "eval",
                    $"localStorage.setItem('bcl-theme', '{theme}')")
                    .ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            // JS interop is unavailable during server-side pre-rendering; the theme
            // is re-applied once InitializeAsync runs after the first render.
        }
        catch (JSException)
        {
            // Browser-side failure (e.g. localStorage blocked). The in-memory theme
            // has already been updated and the ThemeChanged event has been raised.
        }
    }

    private void InvokeThemeChanged(ThemeMode theme)
    {
        lock (_eventLock)
        {
            ThemeChanged?.Invoke(theme);
        }
    }

    private void InvokeOnThemeChanged(string attributeValue)
    {
        lock (_eventLock)
        {
            OnThemeChanged?.Invoke(attributeValue);
        }
    }
}
