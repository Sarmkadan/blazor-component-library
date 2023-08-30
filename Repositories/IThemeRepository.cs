// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// Repository interface for theme persistence.
/// </summary>
public interface IThemeRepository
{
    Task<Theme> CreateAsync(Theme theme);
    Task<Theme?> GetByIdAsync(int id);
    Task<IEnumerable<Theme>> GetAllAsync();
    Task<Theme> UpdateAsync(Theme theme);
    Task<bool> DeleteAsync(int id);
}
