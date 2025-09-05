// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Service for managing application themes and customization.
/// Handles theme CRUD, activation, and CSS variable generation.
/// </summary>
public class ThemeService
{
    private readonly IThemeRepository _themeRepository;

    public ThemeService(IThemeRepository themeRepository)
    {
        _themeRepository = themeRepository ?? throw new ArgumentNullException(nameof(themeRepository));
    }

    /// <summary>
    /// Creates a new theme.
    /// </summary>
    public async Task<Theme> CreateThemeAsync(Theme theme)
    {
        if (theme == null)
            throw new ArgumentNullException(nameof(theme));

        if (!theme.IsValid())
            throw new InvalidOperationException("Theme configuration is invalid");

        if (theme.IsActive)
        {
            await DeactivateAllThemesAsync();
        }

        return await _themeRepository.CreateAsync(theme);
    }

    /// <summary>
    /// Gets a theme by ID.
    /// </summary>
    public async Task<Theme?> GetThemeByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        return await _themeRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Gets all available themes.
    /// </summary>
    public async Task<IEnumerable<Theme>> GetAllThemesAsync()
    {
        return await _themeRepository.GetAllAsync();
    }

    /// <summary>
    /// Gets the currently active theme.
    /// </summary>
    public async Task<Theme?> GetActiveThemeAsync()
    {
        var themes = await _themeRepository.GetAllAsync();
        return themes.FirstOrDefault(t => t.IsActive);
    }

    /// <summary>
    /// Updates a theme.
    /// </summary>
    public async Task<Theme> UpdateThemeAsync(int id, Theme theme)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        if (theme == null)
            throw new ArgumentNullException(nameof(theme));

        if (!theme.IsValid())
            throw new InvalidOperationException("Theme configuration is invalid");

        var existing = await _themeRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Theme with ID {id} not found");

        theme.Id = id;
        theme.CreatedAt = existing.CreatedAt;

        if (theme.IsActive && !existing.IsActive)
        {
            await DeactivateAllThemesAsync();
        }

        return await _themeRepository.UpdateAsync(theme);
    }

    /// <summary>
    /// Activates a theme and deactivates all others.
    /// </summary>
    public async Task<Theme> ActivateThemeAsync(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
            throw new KeyNotFoundException($"Theme with ID {id} not found");

        await DeactivateAllThemesAsync();

        theme.IsActive = true;
        return await _themeRepository.UpdateAsync(theme);
    }

    /// <summary>
    /// Deactivates all themes.
    /// </summary>
    private async Task DeactivateAllThemesAsync()
    {
        var themes = await _themeRepository.GetAllAsync();
        foreach (var theme in themes.Where(t => t.IsActive))
        {
            theme.IsActive = false;
            await _themeRepository.UpdateAsync(theme);
        }
    }

    /// <summary>
    /// Deletes a theme.
    /// </summary>
    public async Task<bool> DeleteThemeAsync(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
            throw new KeyNotFoundException($"Theme with ID {id} not found");

        if (theme.IsActive)
            throw new InvalidOperationException("Cannot delete the active theme");

        return await _themeRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Gets the CSS variables string for the active theme.
    /// </summary>
    public async Task<string> GetActiveCssVariablesAsync()
    {
        var activeTheme = await GetActiveThemeAsync();
        if (activeTheme == null)
            throw new InvalidOperationException("No active theme found");

        return activeTheme.GenerateCssVariables();
    }

    /// <summary>
    /// Gets CSS variables for a specific theme.
    /// </summary>
    public async Task<string> GetCssVariablesAsync(int id)
    {
        var theme = await _themeRepository.GetByIdAsync(id);
        if (theme == null)
            throw new KeyNotFoundException($"Theme with ID {id} not found");

        return theme.GenerateCssVariables();
    }

    /// <summary>
    /// Creates a new theme based on an existing one.
    /// </summary>
    public async Task<Theme> DuplicateThemeAsync(int sourceId, string newName)
    {
        if (sourceId <= 0)
            throw new ArgumentException("Source ID must be greater than 0", nameof(sourceId));

        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("New name cannot be empty", nameof(newName));

        var source = await _themeRepository.GetByIdAsync(sourceId);
        if (source == null)
            throw new KeyNotFoundException($"Theme with ID {sourceId} not found");

        var cloned = source.Clone(newName);
        cloned.IsActive = false;

        return await _themeRepository.CreateAsync(cloned);
    }

    /// <summary>
    /// Gets themes by mode (Light, Dark, Auto).
    /// </summary>
    public async Task<IEnumerable<Theme>> GetThemesByModeAsync(ThemeMode mode)
    {
        var themes = await _themeRepository.GetAllAsync();
        return themes.Where(t => t.Mode == mode);
    }

    /// <summary>
    /// Searches themes by name.
    /// </summary>
    public async Task<IEnumerable<Theme>> SearchThemesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Theme>();

        var themes = await _themeRepository.GetAllAsync();
        var term = searchTerm.ToLower();

        return themes.Where(t => t.Name.ToLower().Contains(term));
    }

    /// <summary>
    /// Gets theme statistics.
    /// </summary>
    public async Task<ThemeStatistics> GetThemeStatisticsAsync()
    {
        var themes = (await _themeRepository.GetAllAsync()).ToList();

        return new ThemeStatistics
        {
            TotalThemes = themes.Count,
            ActiveTheme = themes.FirstOrDefault(t => t.IsActive)?.Name ?? "None",
            LightThemes = themes.Count(t => t.Mode == ThemeMode.Light),
            DarkThemes = themes.Count(t => t.Mode == ThemeMode.Dark),
            LastUpdated = DateTime.UtcNow
        };
    }
}

public class ThemeStatistics
{
    public int TotalThemes { get; set; }
    public string ActiveTheme { get; set; } = string.Empty;
    public int LightThemes { get; set; }
    public int DarkThemes { get; set; }
    public DateTime LastUpdated { get; set; }
}
