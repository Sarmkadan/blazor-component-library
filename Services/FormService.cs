// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Repositories;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Service for managing form configurations and validations.
/// Handles field management and form submission logic.
/// </summary>
public class FormService
{
    private readonly IFormRepository _formRepository;

    public FormService(IFormRepository formRepository)
    {
        _formRepository = formRepository ?? throw new ArgumentNullException(nameof(formRepository));
    }

    /// <summary>
    /// Creates a new form field.
    /// </summary>
    public async Task<FormField> CreateFieldAsync(FormField field)
    {
        if (field == null)
            throw new ArgumentNullException(nameof(field));

        if (string.IsNullOrWhiteSpace(field.Name))
            throw new ArgumentException("Field name is required", nameof(field.Name));

        if (string.IsNullOrWhiteSpace(field.Label))
            throw new ArgumentException("Field label is required", nameof(field.Label));

        return await _formRepository.CreateFieldAsync(field);
    }

    /// <summary>
    /// Gets a form field by ID.
    /// </summary>
    public async Task<FormField?> GetFieldByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        return await _formRepository.GetFieldByIdAsync(id);
    }

    /// <summary>
    /// Gets all form fields.
    /// </summary>
    public async Task<IEnumerable<FormField>> GetAllFieldsAsync()
    {
        return await _formRepository.GetAllFieldsAsync();
    }

    /// <summary>
    /// Gets all form fields ordered by display order.
    /// </summary>
    public async Task<List<FormField>> GetFieldsOrderedAsync()
    {
        var fields = await _formRepository.GetAllFieldsAsync();
        return fields.OrderBy(f => f.Order).ToList();
    }

    /// <summary>
    /// Updates an existing form field.
    /// </summary>
    public async Task<FormField> UpdateFieldAsync(int id, FormField field)
    {
        if (id <= 0)
            throw new ArgumentException("ID must be greater than 0", nameof(id));

        if (field == null)
            throw new ArgumentNullException(nameof(field));

        var existing = await _formRepository.GetFieldByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Field with ID {id} not found");

        field.Id = id;
        return await _formRepository.UpdateFieldAsync(field);
    }

    /// <summary>
    /// Deletes a form field.
    /// </summary>
    public async Task<bool> DeleteFieldAsync(int id)
    {
        return await _formRepository.DeleteFieldAsync(id);
    }

    /// <summary>
    /// Validates form data against all fields.
    /// </summary>
    public async Task<FormValidationResult> ValidateFormAsync(Dictionary<string, object?> formData)
    {
        if (formData == null)
            throw new ArgumentNullException(nameof(formData));

        var fields = await _formRepository.GetAllFieldsAsync();
        var result = new FormValidationResult();

        foreach (var field in fields)
        {
            var value = formData.ContainsKey(field.Name) ? formData[field.Name] : null;
            var validationResult = field.Validate(value);

            if (!validationResult.IsValid)
            {
                result.Errors.Add(field.Name, validationResult.ErrorMessage ?? "Invalid value");
                result.IsValid = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Validates a single field value.
    /// </summary>
    public async Task<ValidationResult> ValidateFieldAsync(string fieldName, object? value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
            throw new ArgumentException("Field name is required", nameof(fieldName));

        var field = (await _formRepository.GetAllFieldsAsync())
            .FirstOrDefault(f => f.Name == fieldName);

        if (field == null)
            throw new KeyNotFoundException($"Field '{fieldName}' not found");

        return field.Validate(value);
    }

    /// <summary>
    /// Gets fields of a specific type.
    /// </summary>
    public async Task<IEnumerable<FormField>> GetFieldsByTypeAsync(FormFieldType fieldType)
    {
        var fields = await _formRepository.GetAllFieldsAsync();
        return fields.Where(f => f.FieldType == fieldType);
    }

    /// <summary>
    /// Updates field order and positions.
    /// </summary>
    public async Task<List<FormField>> ReorderFieldsAsync(List<(int id, int order)> updates)
    {
        if (updates == null || updates.Count == 0)
            throw new ArgumentException("Updates list cannot be empty", nameof(updates));

        var allFields = (await _formRepository.GetAllFieldsAsync()).ToList();

        foreach (var (id, order) in updates)
        {
            var field = allFields.FirstOrDefault(f => f.Id == id);
            if (field != null)
            {
                field.Order = order;
                await _formRepository.UpdateFieldAsync(field);
            }
        }

        return (await _formRepository.GetAllFieldsAsync()).OrderBy(f => f.Order).ToList();
    }

    /// <summary>
    /// Gets required fields.
    /// </summary>
    public async Task<IEnumerable<FormField>> GetRequiredFieldsAsync()
    {
        var fields = await _formRepository.GetAllFieldsAsync();
        return fields.Where(f => f.IsRequired);
    }

    /// <summary>
    /// Gets optional fields.
    /// </summary>
    public async Task<IEnumerable<FormField>> GetOptionalFieldsAsync()
    {
        var fields = await _formRepository.GetAllFieldsAsync();
        return fields.Where(f => !f.IsRequired);
    }

    /// <summary>
    /// Generates a form schema as a dictionary.
    /// </summary>
    public async Task<Dictionary<string, object>> GenerateFormSchemaAsync()
    {
        var fields = (await GetFieldsOrderedAsync()).ToList();
        var schema = new Dictionary<string, object>
        {
            { "fieldCount", fields.Count },
            { "requiredFields", fields.Count(f => f.IsRequired) },
            { "fields", fields.Select(f => new
            {
                f.Name,
                f.Label,
                f.FieldType,
                f.IsRequired,
                f.Order
            }).ToList() }
        };

        return schema;
    }
}

public class FormValidationResult
{
    public bool IsValid { get; set; } = true;
    public Dictionary<string, string> Errors { get; set; } = new();

    public bool HasErrors => Errors.Count > 0;
    public int ErrorCount => Errors.Count;
}
