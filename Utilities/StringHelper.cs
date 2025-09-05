// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text;
using System.Text.RegularExpressions;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Utility class for common string operations.
/// Provides methods for validation, transformation, and manipulation.
/// </summary>
public static class StringHelper
{
    /// <summary>
    /// Converts PascalCase to kebab-case (e.g., MyComponent -> my-component).
    /// Used for CSS class naming and URL slugs.
    /// </summary>
    public static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1-$2").ToLower();
    }

    /// <summary>
    /// Converts PascalCase to snake_case (e.g., MyComponent -> my_component).
    /// Used for database column names and configuration keys.
    /// </summary>
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    /// <summary>
    /// Converts snake_case or kebab-case to PascalCase.
    /// Useful for dynamic property binding and reflection operations.
    /// </summary>
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        var sb = new StringBuilder();

        foreach (var word in words)
        {
            sb.Append(char.ToUpper(word[0]) + word.Substring(1).ToLower());
        }

        return sb.ToString();
    }

    /// <summary>
    /// Validates email format using a basic regex pattern.
    /// For production, consider using System.ComponentModel.DataAnnotations.EmailAddressAttribute.
    /// </summary>
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var emailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    /// <summary>
    /// Validates URL format.
    /// Supports HTTP and HTTPS schemes.
    /// </summary>
    public static bool IsValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Truncates string to specified length and appends ellipsis.
    /// Useful for UI display of long text.
    /// </summary>
    public static string Truncate(string? input, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        if (input.Length <= maxLength)
            return input;

        return input.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Generates a URL-safe slug from arbitrary text.
    /// Removes special characters and replaces spaces with hyphens.
    /// </summary>
    public static string ToUrlSlug(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var slug = input.ToLower();
        slug = Regex.Replace(slug, @"[^\w\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }

    /// <summary>
    /// Sanitizes string to remove potentially harmful characters.
    /// Used for preventing basic injection attacks.
    /// </summary>
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var dangerousChars = new[] { '<', '>', '"', '\'', '&' };
        var sb = new StringBuilder(input);

        foreach (var ch in dangerousChars)
        {
            sb.Replace(ch.ToString(), string.Empty);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Repeats a string a specified number of times.
    /// Equivalent to string * count in some languages.
    /// </summary>
    public static string Repeat(string input, int count)
    {
        if (count <= 0)
            return string.Empty;

        return string.Concat(Enumerable.Repeat(input, count));
    }

    /// <summary>
    /// Checks if string contains only alphanumeric characters.
    /// </summary>
    public static bool IsAlphanumeric(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return input.All(char.IsLetterOrDigit);
    }

    /// <summary>
    /// Counts occurrences of substring within string.
    /// Case-sensitive by default.
    /// </summary>
    public static int CountOccurrences(string input, string searchString, StringComparison comparison = StringComparison.Ordinal)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(searchString))
            return 0;

        int count = 0;
        int index = 0;

        while ((index = input.IndexOf(searchString, index, comparison)) != -1)
        {
            count++;
            index += searchString.Length;
        }

        return count;
    }

    /// <summary>
    /// Removes all whitespace from string.
    /// Useful for data processing and comparison.
    /// </summary>
    public static string RemoveWhitespace(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return Regex.Replace(input, @"\s+", "");
    }

    /// <summary>
    /// Reverses a string character by character.
    /// </summary>
    public static string Reverse(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        return new string(input.Reverse().ToArray());
    }
}
