// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Formatters;

/// <summary>
/// Factory for creating formatter instances based on format type.
/// Centralizes formatter creation and caching.
/// Supports pluggable formatter implementations.
/// </summary>
public class FormatterFactory
{
    private readonly Dictionary<string, Func<object>> _formatters = new();
    private readonly ILogger<FormatterFactory> _logger;

    public FormatterFactory(ILogger<FormatterFactory> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Register default formatters
        RegisterDefaultFormatters();
    }

    /// <summary>
    /// Registers default formatters for common formats.
    /// </summary>
    private void RegisterDefaultFormatters()
    {
        Register("json", () => new JsonFormatter());
        Register("csv", () => new CsvFormatterWrapper());
        Register("xml", () => new XmlFormatterWrapper());
        _logger.LogInformation("Default formatters registered");
    }

    /// <summary>
    /// Registers a custom formatter implementation.
    /// Factory method is called each time formatter is requested.
    /// </summary>
    public void Register(string format, Func<object> factory)
    {
        if (string.IsNullOrEmpty(format))
            throw new ArgumentException("Format cannot be null or empty", nameof(format));

        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        _formatters[format.ToLower()] = factory;
        _logger.LogInformation("Formatter registered: {Format}", format);
    }

    /// <summary>
    /// Gets formatter instance for specified format.
    /// </summary>
    public IDataFormatter GetFormatter(string format)
    {
        if (string.IsNullOrEmpty(format))
            throw new ArgumentException("Format cannot be null or empty", nameof(format));

        var key = format.ToLower();
        if (!_formatters.ContainsKey(key))
            throw new FormattingException($"Formatter not registered for format: {format}");

        try
        {
            var formatter = _formatters[key]();
            if (formatter is not IDataFormatter dataFormatter)
                throw new FormattingException($"Formatter for {format} does not implement IDataFormatter");

            return dataFormatter;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating formatter for format: {Format}", format);
            throw;
        }
    }

    /// <summary>
    /// Serializes object to specified format.
    /// </summary>
    public string Serialize<T>(T? obj, string format) where T : class
    {
        var formatter = GetFormatter(format);
        return formatter.Serialize(obj);
    }

    /// <summary>
    /// Deserializes string from specified format.
    /// </summary>
    public T? Deserialize<T>(string data, string format) where T : class
    {
        var formatter = GetFormatter(format);
        return formatter.Deserialize<T>(data);
    }

    /// <summary>
    /// Converts object from one format to another.
    /// Deserializes from source format, serializes to target format.
    /// </summary>
    public string Convert<T>(T? obj, string sourceFormat, string targetFormat) where T : class
    {
        var json = Serialize(obj, sourceFormat);
        var deserialized = Deserialize<T>(json, sourceFormat);
        return Serialize(deserialized, targetFormat);
    }

    /// <summary>
    /// Exports collection to specified format.
    /// Handles list serialization with proper structure.
    /// </summary>
    public string ExportCollection<T>(IEnumerable<T> items, string format) where T : class
    {
        var formatter = GetFormatter(format);
        var list = items.ToList();

        return format.ToLower() switch
        {
            "csv" => CsvFormatter.ToCsv(list),
            "json" => formatter.Serialize((object)list),
            "xml" => formatter.Serialize((object)list),
            _ => throw new FormattingException($"Export not supported for format: {format}")
        };
    }

    /// <summary>
    /// Imports collection from specified format.
    /// Parses and creates typed objects from formatted data.
    /// </summary>
    public List<T> ImportCollection<T>(string data, string format) where T : class, new()
    {
        return format.ToLower() switch
        {
            "csv" => CsvFormatter.FromCsv<T>(data),
            "json" => JsonConvert.DeserializeObject<List<T>>(data) ?? new List<T>(),
            "xml" => throw new NotImplementedException("XML collection import not yet implemented"),
            _ => throw new FormattingException($"Import not supported for format: {format}")
        };
    }

    /// <summary>
    /// Gets list of supported formats.
    /// </summary>
    public IEnumerable<string> GetSupportedFormats()
    {
        return _formatters.Keys.OrderBy(k => k);
    }

    /// <summary>
    /// Checks if format is supported.
    /// </summary>
    public bool IsFormatSupported(string format)
    {
        return !string.IsNullOrEmpty(format) && _formatters.ContainsKey(format.ToLower());
    }
}

/// <summary>
/// Wrapper for CSV formatter to implement IDataFormatter.
/// </summary>
public class CsvFormatterWrapper : IDataFormatter
{
    public string Serialize<T>(T? obj) where T : class
    {
        if (obj is IEnumerable<T> collection)
            return CsvFormatter.ToCsv(collection);

        throw new FormattingException("CSV formatter requires enumerable collection");
    }

    public T? Deserialize<T>(string data) where T : class
    {
        if (string.IsNullOrEmpty(data))
            return null;

        throw new FormattingException("Single object CSV deserialization not supported");
    }

    public string Format => "csv";
}

/// <summary>
/// Wrapper for XML formatter to implement IDataFormatter.
/// </summary>
public class XmlFormatterWrapper : IDataFormatter
{
    private readonly XmlFormatter _xmlFormatter = new();

    public string Serialize<T>(T? obj) where T : class
    {
        return _xmlFormatter.ToXml(obj);
    }

    public T? Deserialize<T>(string data) where T : class
    {
        return _xmlFormatter.FromXml<T>(data);
    }

    public string Format => "xml";
}

/// <summary>
/// Extension method to register FormatterFactory in dependency injection.
/// </summary>
public static class FormatterFactoryExtensions
{
    public static IServiceCollection AddFormatterFactory(this IServiceCollection services)
    {
        services.AddSingleton<FormatterFactory>();
        return services;
    }

    /// <summary>
    /// Registers custom formatter in factory.
    /// </summary>
    public static IServiceCollection AddCustomFormatter<T>(
        this IServiceCollection services,
        string format) where T : IDataFormatter, new()
    {
        // Register the formatter type for injection
        services.AddTransient<T>();

        // Factory will be configured in FormatterFactory setup
        return services;
    }
}
