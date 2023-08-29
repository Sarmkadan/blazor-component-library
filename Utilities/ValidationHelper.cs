// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Collections.Frozen;
using System.Text.RegularExpressions;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Helper class for common validation operations across the library.
/// Provides reusable validation methods for strings, emails, URLs, and patterns.
/// </summary>
public static class ValidationHelper
{
    // Compiled once — eliminates per-call Regex construction and JIT overhead.
    private static readonly Regex _identifierRegex = new(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
    private static readonly Regex _hexColorRegex = new(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", RegexOptions.Compiled);
    private static readonly Regex _cssClassNameRegex = new(@"^[a-zA-Z_-][a-zA-Z0-9_-]*$", RegexOptions.Compiled);
    private static readonly Regex _sanitizeRegex = new(@"[<>&""']", RegexOptions.Compiled);

    // FrozenDictionary is immutable after construction; its internal layout is optimised for
    // read-heavy workloads and avoids the overhead of a mutable Dictionary bucket scan.
    private static readonly FrozenDictionary<string, string> _validationMessageSuffixes =
        new Dictionary<string, string>
        {
            ["required"]  = "is required",
            ["email"]     = "must be a valid email address",
            ["url"]       = "must be a valid URL",
            ["minLength"] = "is too short",
            ["maxLength"] = "is too long",
            ["pattern"]   = "format is invalid",
            ["range"]     = "is out of range",
        }.ToFrozenDictionary();

    /// <summary>
    /// Validates an email address format.
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a URL format.
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    /// <summary>
    /// Validates that a string contains only alphanumeric characters and underscores.
    /// </summary>
    public static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        return _identifierRegex.IsMatch(identifier);
    }

    /// <summary>
    /// Validates that a string meets minimum length requirements.
    /// </summary>
    public static bool ValidateLength(string? value, int minLength, int maxLength)
    {
        if (value == null)
            return minLength == 0;

        return value.Length >= minLength && value.Length <= maxLength;
    }

    /// <summary>
    /// Validates that a string matches a regex pattern.
    /// </summary>
    public static bool ValidatePattern(string value, string pattern)
    {
        if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            return Regex.IsMatch(value, pattern);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates a color in hex format (#RRGGBB or #RGB).
    /// </summary>
    public static bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        return _hexColorRegex.IsMatch(color);
    }

    /// <summary>
    /// Validates a CSS class name.
    /// </summary>
    public static bool IsValidCssClassName(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return false;

        return _cssClassNameRegex.IsMatch(className);
    }

    /// <summary>
    /// Validates that a value is within a numeric range.
    /// </summary>
    public static bool IsInRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Validates that a value is within a numeric range for decimals.
    /// </summary>
    public static bool IsInRange(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Validates that a value is within a numeric range for doubles.
    /// </summary>
    public static bool IsInRange(double value, double min, double max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Sanitizes a string by removing/escaping potentially harmful characters.
    /// </summary>
    public static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        return _sanitizeRegex.Replace(input, static match => match.Value switch
        {
            "<"  => "&lt;",
            ">"  => "&gt;",
            "&"  => "&amp;",
            "\"" => "&quot;",
            "'"  => "&#39;",
            _    => match.Value
        });
    }

    /// <summary>
    /// Validates that a collection is not null or empty.
    /// </summary>
    public static bool IsNotNullOrEmpty<T>(IEnumerable<T>? collection)
    {
        return collection != null && collection.Any();
    }

    /// <summary>
    /// Validates that all required fields are present in a dictionary.
    /// </summary>
    public static bool ValidateRequiredFields(Dictionary<string, object?> data, params string[] requiredKeys)
    {
        if (data == null || requiredKeys == null)
            return false;

        return requiredKeys.All(key => data.ContainsKey(key) && data[key] != null);
    }

    /// <summary>
    /// Gets validation error message for a field.
    /// </summary>
    public static string GetValidationMessage(string fieldName, string rule)
    {
        return _validationMessageSuffixes.TryGetValue(rule, out var suffix)
            ? $"{fieldName} {suffix}"
            : $"{fieldName} validation failed";
    }
}

/// <summary>
/// Fluent validation builder for composing complex validations.
/// </summary>
public class FluentValidator
{
    private readonly List<string> _errors = new();

    public FluentValidator For(string fieldName)
    {
        _currentFieldName = fieldName;
        return this;
    }

    private string _currentFieldName = string.Empty;

    public FluentValidator Required(object? value)
    {
        if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "required"));
        }
        return this;
    }

    public FluentValidator Email(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && !ValidationHelper.IsValidEmail(value))
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "email"));
        }
        return this;
    }

    public FluentValidator Url(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && !ValidationHelper.IsValidUrl(value))
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "url"));
        }
        return this;
    }

    public FluentValidator MinLength(string? value, int minLength)
    {
        if (value != null && value.Length < minLength)
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "minLength"));
        }
        return this;
    }

    public FluentValidator MaxLength(string? value, int maxLength)
    {
        if (value != null && value.Length > maxLength)
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "maxLength"));
        }
        return this;
    }

    public FluentValidator Pattern(string? value, string pattern)
    {
        if (!string.IsNullOrWhiteSpace(value) && !ValidationHelper.ValidatePattern(value, pattern))
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "pattern"));
        }
        return this;
    }

    public FluentValidator InRange(int value, int min, int max)
    {
        if (!ValidationHelper.IsInRange(value, min, max))
        {
            _errors.Add(ValidationHelper.GetValidationMessage(_currentFieldName, "range"));
        }
        return this;
    }

    public bool IsValid => _errors.Count == 0;
    public List<string> Errors => _errors;

    public void ThrowIfInvalid()
    {
        if (!IsValid)
        {
            throw new InvalidOperationException(string.Join("; ", _errors));
        }
    }
}
