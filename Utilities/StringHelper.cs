// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorComponentLibrary.Utilities;

/// <summary>
/// Utility class for common string operations.
/// Provides methods for validation, transformation, and manipulation.
/// </summary>
public static class StringHelper
{
    // Compiled once at class load; reused across all calls — avoids per-call Regex allocation and JIT compilation.
    private static readonly Regex _camelCaseSeparatorRegex = new(@"([a-z0-9])([A-Z])", RegexOptions.Compiled);
    private static readonly Regex _nonWordCharsRegex = new(@"[^\w\s-]", RegexOptions.Compiled);
    private static readonly Regex _whitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex _multipleDashesRegex = new(@"-+", RegexOptions.Compiled);
    private static readonly Regex _emailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", RegexOptions.Compiled);
    private static readonly Regex _removeWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

    // SearchValues enables SIMD-accelerated single-pass scan for dangerous HTML chars.
    private static readonly SearchValues<char> _dangerousChars = SearchValues.Create("<>\"'&");

    /// <summary>
    /// Converts PascalCase to kebab-case (e.g., MyComponent -> my-component).
    /// Used for CSS class naming and URL slugs.
    /// </summary>
    public static string ToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return _camelCaseSeparatorRegex.Replace(input, "$1-$2").ToLower();
    }

    /// <summary>
    /// Converts PascalCase to snake_case (e.g., MyComponent -> my_component).
    /// Used for database column names and configuration keys.
    /// </summary>
    public static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return _camelCaseSeparatorRegex.Replace(input, "$1_$2").ToLower();
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
        var sb = new StringBuilder(input.Length);

        foreach (var word in words)
        {
            sb.Append(char.ToUpper(word[0]));
            // AsSpan avoids the two intermediate string allocations from Substring(1) + ToLower()
            foreach (var ch in word.AsSpan(1))
                sb.Append(char.ToLower(ch));
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

        return _emailRegex.IsMatch(email);
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
        slug = _nonWordCharsRegex.Replace(slug, "");
        slug = _whitespaceRegex.Replace(slug, "-");
        slug = _multipleDashesRegex.Replace(slug, "-");
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

        // Fast path: SIMD scan — skip allocation entirely when nothing to remove.
        if (input.AsSpan().IndexOfAny(_dangerousChars) < 0)
            return input;

        // Count chars to remove so we can pre-size the result with string.Create.
        var removeCount = 0;
        foreach (var ch in input)
        {
            if (ch is '<' or '>' or '"' or '\'' or '&')
                removeCount++;
        }

        return string.Create(input.Length - removeCount, input, static (span, src) =>
        {
            var pos = 0;
            foreach (var ch in src)
            {
                if (ch is not ('<' or '>' or '"' or '\'' or '&'))
                    span[pos++] = ch;
            }
        });
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

        return _removeWhitespaceRegex.Replace(input, "");
    }

    /// <summary>
    /// Reverses a string character by character.
    /// </summary>
    public static string Reverse(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        // string.Create + Span.Reverse avoids the LINQ ToArray() heap allocation.
        return string.Create(input.Length, input, static (span, src) =>
        {
            src.AsSpan().CopyTo(span);
            span.Reverse();
        });
    }
}
