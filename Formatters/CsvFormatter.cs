// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Reflection;
using System.Text;

namespace BlazorComponentLibrary.Formatters;

/// <summary>
/// CSV formatter for data export and import.
/// Handles complex types and escapes special characters properly.
/// Follows RFC 4180 CSV format standard.
/// </summary>
public class CsvFormatter
{
    private const char DefaultDelimiter = ',';
    private const char DefaultQuote = '"';

    /// <summary>
    /// Converts collection of objects to CSV string.
    /// Automatically handles headers and row formatting.
    /// </summary>
    public static string ToCsv<T>(IEnumerable<T> items, char delimiter = DefaultDelimiter, bool includeHeaders = true) where T : class
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        var sb = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);

        // Write headers
        if (includeHeaders)
        {
            var headers = properties.Select(p => EscapeField(p.Name));
            sb.AppendLine(string.Join(delimiter, headers));
        }

        // Write data rows
        foreach (var item in items)
        {
            var values = properties.Select(p => EscapeField(p.GetValue(item)?.ToString() ?? ""));
            sb.AppendLine(string.Join(delimiter, values));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts CSV string to collection of objects.
    /// Parses headers and maps to object properties.
    /// </summary>
    public static List<T> FromCsv<T>(string csv, char delimiter = DefaultDelimiter, bool hasHeaders = true) where T : class, new()
    {
        if (string.IsNullOrEmpty(csv))
            return new List<T>();

        var lines = csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var result = new List<T>();

        if (lines.Length == 0)
            return result;

        var startIndex = 0;
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase);
        Dictionary<string, PropertyInfo>? propertyMap = null;

        if (hasHeaders)
        {
            var headers = ParseCsvLine(lines[0], delimiter);
            propertyMap = properties
                .Where(p => headers.Contains(p.Name, StringComparer.OrdinalIgnoreCase))
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            startIndex = 1;
        }

        propertyMap ??= properties.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var values = ParseCsvLine(line, delimiter);
            var item = new T();
            var propIndex = 0;

            foreach (var prop in properties)
            {
                if (propIndex < values.Count)
                {
                    try
                    {
                        var value = Convert.ChangeType(values[propIndex], prop.PropertyType);
                        prop.SetValue(item, value);
                    }
                    catch
                    {
                        // Ignore conversion errors
                    }
                }
                propIndex++;
            }

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// Converts CSV string to DataTable for flexible data handling.
    /// Useful when object schema is not known in advance.
    /// </summary>
    public static List<Dictionary<string, string>> ToCsvDictionaries(string csv, char delimiter = DefaultDelimiter, bool hasHeaders = true)
    {
        if (string.IsNullOrEmpty(csv))
            return new List<Dictionary<string, string>>();

        var lines = csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var result = new List<Dictionary<string, string>>();

        if (lines.Length == 0)
            return result;

        var headers = hasHeaders ? ParseCsvLine(lines[0], delimiter) : null;
        var startIndex = hasHeaders ? 1 : 0;

        for (int i = startIndex; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            var values = ParseCsvLine(line, delimiter);
            var row = new Dictionary<string, string>();

            if (headers != null)
            {
                for (int j = 0; j < headers.Count && j < values.Count; j++)
                {
                    row[headers[j]] = values[j];
                }
            }
            else
            {
                for (int j = 0; j < values.Count; j++)
                {
                    row[$"Column{j}"] = values[j];
                }
            }

            result.Add(row);
        }

        return result;
    }

    /// <summary>
    /// Escapes a field value for CSV format.
    /// Wraps fields containing delimiters, quotes, or newlines.
    /// </summary>
    private static string EscapeField(string? field)
    {
        if (string.IsNullOrEmpty(field))
            return string.Empty;

        if (field.Contains(DefaultDelimiter) || field.Contains(DefaultQuote) || field.Contains("\n") || field.Contains("\r"))
        {
            return DefaultQuote + field.Replace(DefaultQuote.ToString(), DefaultQuote.ToString() + DefaultQuote.ToString()) + DefaultQuote;
        }

        return field;
    }

    /// <summary>
    /// Parses a single CSV line respecting quoted fields.
    /// Handles embedded delimiters and quotes correctly.
    /// </summary>
    private static List<string> ParseCsvLine(string line, char delimiter)
    {
        var fields = new List<string>();
        var currentField = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (ch == DefaultQuote)
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == DefaultQuote)
                {
                    currentField.Append(DefaultQuote);
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (ch == delimiter && !inQuotes)
            {
                fields.Add(currentField.ToString());
                currentField.Clear();
            }
            else
            {
                currentField.Append(ch);
            }
        }

        fields.Add(currentField.ToString());
        return fields;
    }
}
