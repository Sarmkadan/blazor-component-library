// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// Repository interface for component configuration persistence.
/// </summary>
public interface IComponentRepository
{
    Task<ComponentConfig> CreateAsync(ComponentConfig component);
    Task<ComponentConfig?> GetByIdAsync(int id);
    Task<IEnumerable<ComponentConfig>> GetAllAsync();
    Task<ComponentConfig> UpdateAsync(ComponentConfig component);
    Task<bool> DeleteAsync(int id);
}
