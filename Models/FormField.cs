// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Represents a form field configuration for the Form component.
/// Includes validation rules, display options, and default values.
/// </summary>
public class FormField
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 1)]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    [JsonProperty("label")]
    public string Label { get; set; } = string.Empty;

    [JsonProperty("fieldType")]
    public FormFieldType FieldType { get; set; } = FormFieldType.Text;

    [JsonProperty("isRequired")]
    public bool IsRequired { get; set; } = false;

    [JsonProperty("isReadOnly")]
    public bool IsReadOnly { get; set; } = false;

    [JsonProperty("placeholder")]
    public string? Placeholder { get; set; }

    [JsonProperty("defaultValue")]
    public string? DefaultValue { get; set; }

    [JsonProperty("minLength")]
    public int? MinLength { get; set; }

    [JsonProperty("maxLength")]
    public int? MaxLength { get; set; }

    [JsonProperty("pattern")]
    public string? Pattern { get; set; }

    [JsonProperty("helpText")]
    public string? HelpText { get; set; }

    [JsonProperty("options")]
    public List<FormFieldOption>? Options { get; set; }

    [JsonProperty("order")]
    public int Order { get; set; } = 0;

    [JsonProperty("cssClass")]
    public string? CssClass { get; set; }

    /// <summary>
    /// Validates a value against the field's constraints.
    /// </summary>
    public ValidationResult Validate(object? value)
    {
        if (IsRequired && (value == null || string.IsNullOrWhiteSpace(value.ToString())))
        {
            return ValidationResult.Error($"{Label} is required");
        }

        if (value == null) return ValidationResult.Success();

        var stringValue = value.ToString() ?? string.Empty;

        if (MinLength.HasValue && stringValue.Length < MinLength.Value)
        {
            return ValidationResult.Error($"{Label} must be at least {MinLength} characters");
        }

        if (MaxLength.HasValue && stringValue.Length > MaxLength.Value)
        {
            return ValidationResult.Error($"{Label} must not exceed {MaxLength} characters");
        }

        if (!string.IsNullOrWhiteSpace(Pattern))
        {
            var regex = new System.Text.RegularExpressions.Regex(Pattern);
            if (!regex.IsMatch(stringValue))
            {
                return ValidationResult.Error($"{Label} format is invalid");
            }
        }

        if (FieldType == FormFieldType.Email && !stringValue.Contains("@"))
        {
            return ValidationResult.Error($"{Label} must be a valid email address");
        }

        if (FieldType == FormFieldType.Url && !IsValidUrl(stringValue))
        {
            return ValidationResult.Error($"{Label} must be a valid URL");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Checks if a string is a valid URL.
    /// </summary>
    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    /// <summary>
    /// Gets the option by value.
    /// </summary>
    public FormFieldOption? GetOption(string value)
    {
        return Options?.FirstOrDefault(o => o.Value == value);
    }

    /// <summary>
    /// Creates a copy of this field.
    /// </summary>
    public FormField Copy()
    {
        return new FormField
        {
            Id = Id,
            Name = Name,
            Label = Label,
            FieldType = FieldType,
            IsRequired = IsRequired,
            IsReadOnly = IsReadOnly,
            Placeholder = Placeholder,
            DefaultValue = DefaultValue,
            MinLength = MinLength,
            MaxLength = MaxLength,
            Pattern = Pattern,
            HelpText = HelpText,
            Options = Options?.Select(o => new FormFieldOption { Value = o.Value, Label = o.Label }).ToList(),
            Order = Order,
            CssClass = CssClass
        };
    }
}

public class FormFieldOption
{
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;

    [JsonProperty("label")]
    public string Label { get; set; } = string.Empty;
}

public enum FormFieldType
{
    Text = 0,
    Email = 1,
    Password = 2,
    Number = 3,
    Date = 4,
    Time = 5,
    Textarea = 6,
    Select = 7,
    Checkbox = 8,
    Radio = 9,
    File = 10,
    Url = 11,
    Tel = 12
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Error(string message) => new() { IsValid = false, ErrorMessage = message };
}
