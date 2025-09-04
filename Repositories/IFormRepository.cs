// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Repositories;

/// <summary>
/// Repository interface for form field persistence.
/// </summary>
public interface IFormRepository
{
    Task<FormField> CreateFieldAsync(FormField field);
    Task<FormField?> GetFieldByIdAsync(int id);
    Task<IEnumerable<FormField>> GetAllFieldsAsync();
    Task<FormField> UpdateFieldAsync(FormField field);
    Task<bool> DeleteFieldAsync(int id);
}
