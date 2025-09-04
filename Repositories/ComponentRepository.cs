// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// In-memory implementation of the component repository.
/// In production, this would be replaced with a database implementation.
/// </summary>
public class ComponentRepository : IComponentRepository
{
    private readonly List<ComponentConfig> _components = new();
    private int _nextId = 1;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        component.Id = _nextId++;
        component.CreatedAt = DateTime.UtcNow;
        _components.Add(component);

        return await Task.FromResult(component);
    }

    public async Task<ComponentConfig?> GetByIdAsync(int id)
    {
        var component = _components.FirstOrDefault(c => c.Id == id);
        return await Task.FromResult(component);
    }

    public async Task<IEnumerable<ComponentConfig>> GetAllAsync()
    {
        return await Task.FromResult(_components.AsEnumerable());
    }

    public async Task<ComponentConfig> UpdateAsync(ComponentConfig component)
    {
        if (component == null)
            throw new ArgumentNullException(nameof(component));

        var existing = _components.FirstOrDefault(c => c.Id == component.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Component with ID {component.Id} not found");

        var index = _components.IndexOf(existing);
        _components[index] = component;

        return await Task.FromResult(component);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var component = _components.FirstOrDefault(c => c.Id == id);
        if (component == null)
            return await Task.FromResult(false);

        _components.Remove(component);
        return await Task.FromResult(true);
    }
}
