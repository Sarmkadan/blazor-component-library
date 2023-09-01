// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// In-memory implementation of the theme repository.
/// Manages application themes and customizations.
/// </summary>
public class ThemeRepository : IThemeRepository
{
    private readonly List<Theme> _themes = new();
    private int _nextId = 1;

    /// <summary>
    /// Initializes the repository with default light and dark themes.
    /// </summary>
    public ThemeRepository()
    {
        InitializeDefaultThemes();
    }

    private void InitializeDefaultThemes()
    {
        var lightTheme = new Theme
        {
            Id = _nextId++,
            Name = "Light",
            Mode = ThemeMode.Light,
            PrimaryColor = "#007bff",
            SecondaryColor = "#6c757d",
            SuccessColor = "#28a745",
            DangerColor = "#dc3545",
            WarningColor = "#ffc107",
            InfoColor = "#17a2b8",
            BackgroundColor = "#ffffff",
            TextColor = "#212529",
            BorderColor = "#dee2e6",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var darkTheme = new Theme
        {
            Id = _nextId++,
            Name = "Dark",
            Mode = ThemeMode.Dark,
            PrimaryColor = "#0d6efd",
            SecondaryColor = "#adb5bd",
            SuccessColor = "#198754",
            DangerColor = "#dc3545",
            WarningColor = "#ffc107",
            InfoColor = "#0dcaf0",
            BackgroundColor = "#212529",
            TextColor = "#e9ecef",
            BorderColor = "#495057",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        _themes.Add(lightTheme);
        _themes.Add(darkTheme);
    }

    public async Task<Theme> CreateAsync(Theme theme)
    {
        if (theme == null)
            throw new ArgumentNullException(nameof(theme));

        theme.Id = _nextId++;
        theme.CreatedAt = DateTime.UtcNow;
        _themes.Add(theme);

        return await Task.FromResult(theme);
    }

    public async Task<Theme?> GetByIdAsync(int id)
    {
        return await Task.FromResult(_themes.FirstOrDefault(t => t.Id == id));
    }

    public async Task<IEnumerable<Theme>> GetAllAsync()
    {
        return await Task.FromResult(_themes.AsEnumerable());
    }

    public async Task<Theme> UpdateAsync(Theme theme)
    {
        if (theme == null)
            throw new ArgumentNullException(nameof(theme));

        var existing = _themes.FirstOrDefault(t => t.Id == theme.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Theme with ID {theme.Id} not found");

        var index = _themes.IndexOf(existing);
        _themes[index] = theme;

        return await Task.FromResult(theme);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var theme = _themes.FirstOrDefault(t => t.Id == id);
        if (theme == null)
            return await Task.FromResult(false);

        _themes.Remove(theme);
        return await Task.FromResult(true);
    }
}
