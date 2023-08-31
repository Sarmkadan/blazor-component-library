// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Manages dark mode preferences and CSS variable generation per user.
/// Merges the active base theme's accent colours with dark-mode surface overrides
/// so that switching themes and toggling dark mode are fully independent concerns.
/// </summary>
public class DarkModeService
{
    private readonly IThemeRepository _themeRepository;
    private readonly ILogger<DarkModeService> _logger;
    private readonly Dictionary<string, DarkModePreference> _preferences = new(StringComparer.OrdinalIgnoreCase);

    public DarkModeService(IThemeRepository themeRepository, ILogger<DarkModeService> logger)
    {
        _themeRepository = themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Returns the dark mode preference for <paramref name="userId"/>.
    /// A default preference (light mode, follow-system) is returned when none has been saved.
    /// </summary>
    public Task<DarkModePreference> GetPreferenceAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        cancellationToken.ThrowIfCancellationRequested();

        _logger.LogInformation("Retrieving dark mode preference for user {UserId}", userId);

        if (_preferences.TryGetValue(userId, out var saved))
            return Task.FromResult(saved);

        return Task.FromResult(new DarkModePreference { UserId = userId });
    }

    /// <summary>
    /// Persists <paramref name="preference"/> and returns the saved instance
    /// with an updated <c>UpdatedAt</c> timestamp.
    /// </summary>
    public Task<DarkModePreference> SavePreferenceAsync(DarkModePreference preference, CancellationToken cancellationToken = default)
    {
        if (preference == null)
            throw new ArgumentNullException(nameof(preference));

        if (!preference.IsValid())
            throw new InvalidOperationException("Dark mode preference is invalid.");

        cancellationToken.ThrowIfCancellationRequested();

        preference.UpdatedAt = DateTime.UtcNow;
        _preferences[preference.UserId] = preference;

        _logger.LogInformation(
            "Dark mode preference saved for user {UserId}: IsDarkMode={IsDarkMode}, FollowSystem={FollowSystem}",
            preference.UserId, preference.IsDarkMode, preference.FollowSystemPreference);

        return Task.FromResult(preference);
    }

    /// <summary>
    /// Flips the <see cref="DarkModePreference.IsDarkMode"/> flag for <paramref name="userId"/>
    /// and disables system-preference following so the explicit choice is respected.
    /// </summary>
    public async Task<DarkModePreference> ToggleDarkModeAsync(string userId, CancellationToken cancellationToken = default)
    {
        var preference = await GetPreferenceAsync(userId, cancellationToken);

        preference.IsDarkMode = !preference.IsDarkMode;
        preference.FollowSystemPreference = false;

        _logger.LogInformation(
            "Toggled dark mode for user {UserId}: now {Mode}",
            userId, preference.IsDarkMode ? "dark" : "light");

        return await SavePreferenceAsync(preference, cancellationToken);
    }

    /// <summary>
    /// Builds the full CSS block to inject into the page for the given user.
    /// Includes: base theme variables, dark-mode surface overrides, and transition rules.
    /// Intended for use inside an inline <c>&lt;style&gt;</c> element in Blazor layouts.
    /// </summary>
    public async Task<string> GetDarkModeCssAsync(string userId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var preference = await GetPreferenceAsync(userId, cancellationToken);
        var themes = await _themeRepository.GetAllAsync();
        var activeTheme = themes.FirstOrDefault(t => t.IsActive);

        _logger.LogInformation("Generating dark mode CSS for user {UserId}", userId);

        var parts = new List<string>();

        if (activeTheme != null)
            parts.Add(activeTheme.GenerateCssVariables());

        parts.Add(preference.GenerateDarkCssVariables());
        parts.Add(preference.GenerateTransitionCss());

        return string.Join(Environment.NewLine + Environment.NewLine, parts);
    }

    /// <summary>
    /// Returns the HTML attribute string to set on the root <c>&lt;html&gt;</c> element.
    /// Example output: <c>data-theme="dark"</c> or <c>data-theme="light"</c>.
    /// </summary>
    public async Task<string> GetRootThemeAttributeAsync(string userId, CancellationToken cancellationToken = default)
    {
        var preference = await GetPreferenceAsync(userId, cancellationToken);
        return preference.IsDarkMode ? "dark" : "light";
    }

    /// <summary>
    /// Returns all stored preferences. Useful for admin dashboards or bulk migrations.
    /// </summary>
    public Task<IEnumerable<DarkModePreference>> GetAllPreferencesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<DarkModePreference>>(_preferences.Values.ToList());
    }

    /// <summary>
    /// Removes the stored preference for <paramref name="userId"/>, restoring default behaviour.
    /// Returns <c>true</c> when a preference was found and removed, <c>false</c> otherwise.
    /// </summary>
    public Task<bool> ResetPreferenceAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        cancellationToken.ThrowIfCancellationRequested();

        var removed = _preferences.Remove(userId);

        _logger.LogInformation("Dark mode preference reset for user {UserId}", userId);

        return Task.FromResult(removed);
    }
}
