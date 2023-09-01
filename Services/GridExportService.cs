// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Text;
using System.Text.Json;
using BlazorComponentLibrary.Models;
using Microsoft.Extensions.Logging;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Contract for serialising a complete, filtered, and sorted grid result set to a UTF-8
/// byte payload suitable for HTTP file download.
/// </summary>
public interface IGridExportService
{
    /// <summary>
    /// Lower-case format identifiers supported by this service (e.g., <c>csv</c>, <c>json</c>,
    /// <c>xml</c>).  Used by the controller to validate the requested format before calling
    /// <see cref="ExportAsync"/>.
    /// </summary>
    IReadOnlyList<string> SupportedFormats { get; }

    /// <summary>
    /// Retrieves the complete filtered and sorted row set for <paramref name="tableId"/> and
    /// serialises it to the requested <paramref name="format"/>.
    /// <para>
    /// The <see cref="GridVirtualRequest.StartIndex"/> and <see cref="GridVirtualRequest.Count"/>
    /// in <paramref name="request"/> are overridden internally to page through the full filtered
    /// set rather than a single display window.  All other query parameters (filters, sorts,
    /// search term) are preserved exactly.
    /// </para>
    /// </summary>
    /// <param name="tableId">Identifier of the source data table.</param>
    /// <param name="request">
    ///   Virtual query parameters that determine which rows are included and in what order.
    /// </param>
    /// <param name="format">
    ///   Case-insensitive output format key — one of the values in <see cref="SupportedFormats"/>.
    /// </param>
    /// <param name="cancellationToken">Token to observe for cancellation between page fetches.</param>
    /// <returns>UTF-8 encoded byte array of the serialised data.</returns>
    Task<byte[]> ExportAsync(
        int tableId,
        GridVirtualRequest request,
        string format,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Default <see cref="IGridExportService"/> implementation.
/// <para>
/// Fetches the full filtered row set by issuing successive 500-row page requests through
/// <see cref="IVirtualizedGridService.QueryAsync"/>, which ensures that row-level business
/// logic, security rules, and the distributed cache are all respected during export — the
/// export pipeline is never a bypass route.
/// </para>
/// <para>
/// Three serialisation formats are supported:
/// <list type="bullet">
///   <item>
///     <b>csv</b> — RFC 4180-compliant comma-separated values with a header row derived from
///     the first row's <see cref="DataTableRow.Data"/> keys.  Fields containing commas, quotes,
///     or newlines are double-quoted per the specification.
///   </item>
///   <item>
///     <b>json</b> — Pretty-printed JSON array of cell-value dictionaries keyed by column name.
///   </item>
///   <item>
///     <b>xml</b> — Indented XML document with a <c>&lt;rows&gt;</c> root element and one
///     <c>&lt;row&gt;</c> child per data row.  Column names are sanitised into valid XML element
///     names and special characters in values are XML-escaped.
///   </item>
/// </list>
/// </para>
/// </summary>
public sealed class GridExportService : IGridExportService
{
    private const int ExportPageSize = 500;

    private readonly IVirtualizedGridService _gridService;
    private readonly ILogger<GridExportService> _logger;

    /// <inheritdoc />
    public IReadOnlyList<string> SupportedFormats { get; } = ["csv", "json", "xml"];

    /// <summary>
    /// Initialises a new <see cref="GridExportService"/>.
    /// </summary>
    /// <param name="gridService">
    ///   Virtualized grid service used to page through the full filtered and sorted row set.
    /// </param>
    /// <param name="logger">Structured logger for export telemetry.</param>
    public GridExportService(
        IVirtualizedGridService gridService,
        ILogger<GridExportService> logger)
    {
        _gridService = gridService ?? throw new ArgumentNullException(nameof(gridService));
        _logger      = logger      ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<byte[]> ExportAsync(
        int tableId,
        GridVirtualRequest request,
        string format,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var rows = await FetchAllRowsAsync(tableId, request, cancellationToken);

        _logger.LogInformation(
            "Exporting {Count} rows from table {TableId} in format '{Format}'.",
            rows.Count, tableId, format);

        return format.ToLowerInvariant() switch
        {
            "csv"  => Encoding.UTF8.GetBytes(SerializeAsCsv(rows)),
            "json" => SerializeAsJson(rows),
            "xml"  => Encoding.UTF8.GetBytes(SerializeAsXml(rows)),
            _      => throw new NotSupportedException(
                          $"Export format '{format}' is not supported by {nameof(GridExportService)}.")
        };
    }

    // ── Row fetching ──────────────────────────────────────────────────────────

    private async Task<IReadOnlyList<DataTableRow>> FetchAllRowsAsync(
        int tableId,
        GridVirtualRequest request,
        CancellationToken cancellationToken)
    {
        var allRows    = new List<DataTableRow>();
        int startIndex = 0;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var page = await _gridService.QueryAsync(
                tableId,
                request with { StartIndex = startIndex, Count = ExportPageSize },
                cancellationToken);

            allRows.AddRange(page.Items);

            if (!page.HasMore || page.Items.Count == 0)
                break;

            startIndex += page.Items.Count;
        }

        return allRows;
    }

    // ── Serialisation ─────────────────────────────────────────────────────────

    private static string SerializeAsCsv(IReadOnlyList<DataTableRow> rows)
    {
        if (rows.Count == 0)
            return string.Empty;

        var headers = rows[0].Data.Keys.ToList();
        var sb      = new StringBuilder();

        // Header row
        sb.AppendLine(string.Join(",", headers.Select(EscapeCsvField)));

        // Data rows
        foreach (var row in rows)
        {
            var values = headers.Select(h =>
                EscapeCsvField(
                    row.Data.TryGetValue(h, out var v) ? v?.ToString() ?? string.Empty : string.Empty));

            sb.AppendLine(string.Join(",", values));
        }

        return sb.ToString();
    }

    private static byte[] SerializeAsJson(IReadOnlyList<DataTableRow> rows)
    {
        var dataRows = rows.Select(r => r.Data).ToList();

        return JsonSerializer.SerializeToUtf8Bytes(
            dataRows,
            new JsonSerializerOptions { WriteIndented = true });
    }

    private static string SerializeAsXml(IReadOnlyList<DataTableRow> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<rows>");

        foreach (var row in rows)
        {
            sb.AppendLine("  <row>");
            foreach (var (key, value) in row.Data)
            {
                var elementName = XmlSafeElementName(key);
                sb.AppendLine(
                    $"    <{elementName}>{EscapeXmlValue(value?.ToString())}</{elementName}>");
            }
            sb.AppendLine("  </row>");
        }

        sb.AppendLine("</rows>");
        return sb.ToString();
    }

    // ── Encoding helpers ──────────────────────────────────────────────────────

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
            return $"\"{field.Replace("\"", "\"\"")}\"";

        return field;
    }

    private static string EscapeXmlValue(string? value)
    {
        if (value is null) return string.Empty;

        return value
            .Replace("&",  "&amp;")
            .Replace("<",  "&lt;")
            .Replace(">",  "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'",  "&apos;");
    }

    /// <summary>
    /// Converts an arbitrary column key into a valid XML element name by replacing disallowed
    /// characters with underscores and prepending an underscore when the first character is a digit.
    /// </summary>
    private static string XmlSafeElementName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "field";

        var safe = new string(name
            .Select((c, i) => char.IsLetterOrDigit(c) || c == '_' || (i > 0 && c == '-') ? c : '_')
            .ToArray());

        return char.IsLetter(safe[0]) || safe[0] == '_' ? safe : "_" + safe;
    }
}
