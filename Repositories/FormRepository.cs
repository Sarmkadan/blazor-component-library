// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// In-memory implementation of the form repository.
/// Manages form field configurations.
/// </summary>
public class FormRepository : IFormRepository
{
    private readonly List<FormField> _fields = new();
    private int _nextId = 1;

    public async Task<FormField> CreateFieldAsync(FormField field)
    {
        if (field == null)
            throw new ArgumentNullException(nameof(field));

        field.Id = _nextId++;
        _fields.Add(field);

        return await Task.FromResult(field);
    }

    public async Task<FormField?> GetFieldByIdAsync(int id)
    {
        return await Task.FromResult(_fields.FirstOrDefault(f => f.Id == id));
    }

    public async Task<IEnumerable<FormField>> GetAllFieldsAsync()
    {
        return await Task.FromResult(_fields.AsEnumerable());
    }

    public async Task<FormField> UpdateFieldAsync(FormField field)
    {
        if (field == null)
            throw new ArgumentNullException(nameof(field));

        var existing = _fields.FirstOrDefault(f => f.Id == field.Id);
        if (existing == null)
            throw new KeyNotFoundException($"Field with ID {field.Id} not found");

        var index = _fields.IndexOf(existing);
        _fields[index] = field;

        return await Task.FromResult(field);
    }

    public async Task<bool> DeleteFieldAsync(int id)
    {
        var field = _fields.FirstOrDefault(f => f.Id == id);
        if (field == null)
            return await Task.FromResult(false);

        _fields.Remove(field);
        return await Task.FromResult(true);
    }
}
