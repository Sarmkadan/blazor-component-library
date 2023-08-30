// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Service for managing component configurations and lifecycle.
/// Handles CRUD operations and component-related business logic.
/// </summary>
public class ComponentService
{
    private readonly IComponentRepository _componentRepository;

    public ComponentService(IComponentRepository componentRepository)
    {
        _componentRepository = componentRepository ?? throw new ArgumentNullException(nameof(componentRepository));
    }

    /// <summary>
    /// Creates a new component configuration in the repository.
    /// </summary>
    public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (!config.IsValid())
            throw new InvalidOperationException("Component configuration is invalid");

        config.CreatedAt = DateTime.UtcNow;
        return await _componentRepository.CreateAsync(config);
    }

    /// <summary>
    /// Retrieves a component by ID.
    /// </summary>
    public async Task<ComponentConfig?> GetComponentByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        return await _componentRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Gets all active components, optionally filtered by type.
    /// </summary>
    public async Task<IEnumerable<ComponentConfig>> GetAllComponentsAsync(string? componentType = null)
    {
        var components = await _componentRepository.GetAllAsync();
        var result = components.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(componentType))
        {
            result = result.Where(c => c.ComponentType == componentType);
        }

        return result.OrderBy(c => c.DisplayOrder);
    }

    /// <summary>
    /// Gets components by type.
    /// </summary>
    public async Task<IEnumerable<ComponentConfig>> GetComponentsByTypeAsync(string componentType)
    {
        if (string.IsNullOrWhiteSpace(componentType))
            throw new ArgumentException("Component type cannot be empty", nameof(componentType));

        var components = await _componentRepository.GetAllAsync();
        return components.Where(c => c.ComponentType == componentType && c.IsActive);
    }

    /// <summary>
    /// Updates an existing component configuration.
    /// </summary>
    public async Task<ComponentConfig> UpdateComponentAsync(int id, ComponentConfig config)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (!config.IsValid())
            throw new InvalidOperationException("Component configuration is invalid");

        var existing = await _componentRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Component with ID {id} not found");

        config.Id = id;
        config.CreatedAt = existing.CreatedAt;
        config.ModifiedAt = DateTime.UtcNow;

        return await _componentRepository.UpdateAsync(config);
    }

    /// <summary>
    /// Soft deletes a component by marking it as inactive.
    /// </summary>
    public async Task<bool> DeleteComponentAsync(int id)
    {
        var component = await _componentRepository.GetByIdAsync(id);
        if (component == null)
            throw new KeyNotFoundException($"Component with ID {id} not found");

        component.IsActive = false;
        component.ModifiedAt = DateTime.UtcNow;
        await _componentRepository.UpdateAsync(component);
        return true;
    }

    /// <summary>
    /// Searches components by name.
    /// </summary>
    public async Task<IEnumerable<ComponentConfig>> SearchComponentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<ComponentConfig>();

        var components = await _componentRepository.GetAllAsync();
        var term = searchTerm.ToLower();

        return components.Where(c =>
            c.IsActive &&
            (c.Name.ToLower().Contains(term) || c.Description.ToLower().Contains(term))
        );
    }

    /// <summary>
    /// Updates component display order and returns sorted list.
    /// </summary>
    public async Task<IEnumerable<ComponentConfig>> UpdateComponentOrderAsync(List<(int id, int order)> updates)
    {
        if (updates == null || updates.Count == 0)
            throw new ArgumentException("Updates list cannot be empty", nameof(updates));

        var allComponents = await _componentRepository.GetAllAsync();

        foreach (var (id, order) in updates)
        {
            var component = allComponents.FirstOrDefault(c => c.Id == id);
            if (component != null)
            {
                component.DisplayOrder = order;
                component.ModifiedAt = DateTime.UtcNow;
                await _componentRepository.UpdateAsync(component);
            }
        }

        return (await _componentRepository.GetAllAsync()).OrderBy(c => c.DisplayOrder);
    }

    /// <summary>
    /// Gets component usage statistics.
    /// </summary>
    public async Task<ComponentStatistics> GetComponentStatisticsAsync()
    {
        var components = await _componentRepository.GetAllAsync();
        var activeComponents = components.Where(c => c.IsActive).ToList();

        return new ComponentStatistics
        {
            TotalComponents = components.Count,
            ActiveComponents = activeComponents.Count,
            InactiveComponents = components.Count(c => !c.IsActive),
            ComponentsByType = components
                .GroupBy(c => c.ComponentType)
                .ToDictionary(g => g.Key, g => g.Count()),
            LastUpdated = DateTime.UtcNow
        };
    }
}

public class ComponentStatistics
{
    public int TotalComponents { get; set; }
    public int ActiveComponents { get; set; }
    public int InactiveComponents { get; set; }
    public Dictionary<string, int> ComponentsByType { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}
