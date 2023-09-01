// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorComponentLibrary.Models;

/// <summary>
/// Specifies how cells or rows behave when a user initiates an edit in the virtualized grid.
/// </summary>
public enum GridEditMode
{
    /// <summary>All editing is disabled; the grid is read-only.</summary>
    None,

    /// <summary>
    /// A single clicked cell enters an in-place editor. Other cells remain read-only until
    /// the active edit is committed or cancelled.
    /// </summary>
    SingleCell,

    /// <summary>
    /// Clicking any cell in a row places the entire row into edit mode simultaneously,
    /// rendering an editor for every editable column at once.
    /// </summary>
    FullRow
}

/// <summary>
/// Governs how many rows can be selected at one time in the virtualized grid.
/// </summary>
public enum GridSelectionMode
{
    /// <summary>Row selection is entirely disabled.</summary>
    None,

    /// <summary>At most one row is selected at a time; selecting a new row deselects the previous one.</summary>
    Single,

    /// <summary>Any number of rows may be selected concurrently via checkboxes or Ctrl+click.</summary>
    Multiple
}

/// <summary>
/// Parameters that control the virtual scrolling window rendered by the grid.
/// The combination of <see cref="RowHeight"/> and <see cref="OverscanCount"/> determines
/// how many DOM nodes are kept alive while the user scrolls.
/// </summary>
/// <param name="RowHeight">Fixed height of every rendered row in pixels (16–200).</param>
/// <param name="OverscanCount">Extra rows rendered above and below the visible viewport to prevent blank flicker.</param>
/// <param name="PageSize">Rows requested per server round-trip when the scroll position advances past the current window.</param>
public record VirtualScrollConfig(
    [property: Range(16, 200)] int RowHeight    = 40,
    [property: Range(0,  20)]  int OverscanCount = 5,
    [property: Range(10, 500)] int PageSize      = 50
);

/// <summary>
/// Top-level configuration bag passed to <see cref="Services.IVirtualizedGridService"/> at
/// registration time.  All properties are mutable to support the
/// <c>Action&lt;VirtualizedGridOptions&gt;</c> configuration pattern used in
/// <c>AddVirtualizedGrid()</c>.
/// </summary>
public sealed class VirtualizedGridOptions
{
    /// <summary>
    /// Virtual-scroll tuning parameters.
    /// Defaults to 40 px row height, 5 overscan rows, and 50-row server pages.
    /// </summary>
    [JsonPropertyName("virtualScroll")]
    public VirtualScrollConfig VirtualScroll { get; set; } = new();

    /// <summary>Inline editing behaviour. Defaults to <see cref="GridEditMode.None"/> (read-only grid).</summary>
    [JsonPropertyName("editMode")]
    public GridEditMode EditMode { get; set; } = GridEditMode.None;

    /// <summary>Row selection behaviour. Defaults to <see cref="GridSelectionMode.Single"/>.</summary>
    [JsonPropertyName("selectionMode")]
    public GridSelectionMode SelectionMode { get; set; } = GridSelectionMode.Single;

    /// <summary>When <c>true</c>, users may drag column dividers to resize columns.</summary>
    [JsonPropertyName("enableColumnResizing")]
    public bool EnableColumnResizing { get; set; } = true;

    /// <summary>When <c>true</c>, columns may be reordered by dragging their header cell.</summary>
    [JsonPropertyName("enableColumnReordering")]
    public bool EnableColumnReordering { get; set; } = false;

    /// <summary>When <c>true</c>, a toolbar export button is rendered supporting CSV, JSON, and XML.</summary>
    [JsonPropertyName("enableExport")]
    public bool EnableExport { get; set; } = true;

    /// <summary>When <c>true</c>, row grouping by a designated column is available.</summary>
    [JsonPropertyName("enableGrouping")]
    public bool EnableGrouping { get; set; } = false;

    /// <summary>Maximum number of simultaneously active column filters (1–20).</summary>
    [Range(1, 20)]
    [JsonPropertyName("maxFilterCount")]
    public int MaxFilterCount { get; set; } = 10;

    /// <summary>
    /// How long computed query windows are stored in the distributed cache.
    /// Set to <see cref="TimeSpan.Zero"/> to disable result caching.
    /// </summary>
    [JsonPropertyName("cacheExpiration")]
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>Enable caching of filtered/sorted result sets to avoid repeated computation on repeated scrolls.</summary>
    [JsonPropertyName("enableCaching")]
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Maximum edit operations retained per table in the undo stack (0–200).
    /// Older entries are evicted in FIFO order when the limit is reached.
    /// </summary>
    [Range(0, 200)]
    [JsonPropertyName("maxInlineEditHistory")]
    public int MaxInlineEditHistory { get; set; } = 50;

    /// <summary>Render a loading-skeleton overlay while the first data window is being fetched.</summary>
    [JsonPropertyName("showLoadingSkeleton")]
    public bool ShowLoadingSkeleton { get; set; } = true;

    /// <summary>
    /// Debounce delay in milliseconds applied to live filter and search inputs to throttle
    /// server round-trips while the user is typing (0–2000).
    /// </summary>
    [Range(0, 2000)]
    [JsonPropertyName("filterDebounceMs")]
    public int FilterDebounceMs { get; set; } = 300;
}

/// <summary>
/// Extends <see cref="DataTableColumn"/> with inline-editing constraints and advanced display
/// options specific to the virtualized grid.  Instances are stored per-table inside
/// <see cref="Services.IVirtualizedGridService"/> and govern both client-side rendering hints
/// and server-side validation rules applied before any edit is committed.
/// </summary>
public sealed class GridColumnDefinition : DataTableColumn
{
    /// <summary>
    /// When <c>true</c>, this column's cells may be edited inline according to the grid's
    /// <see cref="VirtualizedGridOptions.EditMode"/>.  Columns with <c>IsEditable = false</c>
    /// are always rendered as read-only regardless of the edit mode.
    /// </summary>
    [JsonPropertyName("isEditable")]
    public bool IsEditable { get; set; }

    /// <summary>
    /// Optional .NET regular-expression pattern the edited string value must satisfy before
    /// the edit is committed.  The pattern is matched against the full string value.
    /// </summary>
    [JsonPropertyName("validationRegex")]
    public string? ValidationRegex { get; set; }

    /// <summary>Inclusive lower bound for numeric and date column values.</summary>
    [JsonPropertyName("minValue")]
    public object? MinValue { get; set; }

    /// <summary>Inclusive upper bound for numeric and date column values.</summary>
    [JsonPropertyName("maxValue")]
    public object? MaxValue { get; set; }

    /// <summary>When <c>true</c>, an empty or <c>null</c> value is rejected during inline editing.</summary>
    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    /// <summary>Maximum character length accepted by the inline editor for string columns.</summary>
    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    /// <summary>Placeholder text shown in the inline editor input when the current cell value is empty.</summary>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>
    /// Pins this column to the leading edge of the grid so it remains visible while the
    /// user scrolls horizontally through wide datasets.
    /// </summary>
    [JsonPropertyName("isFrozen")]
    public bool IsFrozen { get; set; }

    /// <summary>
    /// Display order among frozen columns — lower values appear closer to the leading edge.
    /// Ignored when <see cref="IsFrozen"/> is <c>false</c>.
    /// </summary>
    [JsonPropertyName("frozenPosition")]
    public int FrozenPosition { get; set; }

    /// <summary>
    /// Logical group name used when column grouping headers are rendered.
    /// Columns that share the same key are visually grouped under one spanning header.
    /// </summary>
    [JsonPropertyName("groupKey")]
    public string? GroupKey { get; set; }

    /// <summary>
    /// Maps raw cell values to user-friendly display strings (e.g., status codes to localised labels).
    /// The lookup is applied only for rendering; the underlying raw value is always stored and queried.
    /// </summary>
    [JsonPropertyName("displayValueMap")]
    public Dictionary<string, string>? DisplayValueMap { get; set; }

    /// <summary>
    /// Validates a candidate value against all constraints declared on this column definition.
    /// Called by the service layer before any edit is committed to the repository.
    /// </summary>
    /// <param name="value">The proposed new value supplied by the inline editor.</param>
    /// <returns>
    /// <c>true</c> when the value satisfies every active constraint; <c>false</c> otherwise.
    /// </returns>
    public bool ValidateValue(object? value)
    {
        if (IsRequired && value is null or "")
            return false;

        if (MaxLength.HasValue && value is string s && s.Length > MaxLength.Value)
            return false;

        if (ValidationRegex is { Length: > 0 } pattern && value is string text)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(text, pattern))
                return false;
        }

        if ((MinValue is not null || MaxValue is not null) && value is IComparable comparable)
        {
            try
            {
                if (MinValue is not null)
                {
                    var min = (IComparable)Convert.ChangeType(MinValue, value.GetType());
                    if (comparable.CompareTo(min) < 0) return false;
                }

                if (MaxValue is not null)
                {
                    var max = (IComparable)Convert.ChangeType(MaxValue, value.GetType());
                    if (comparable.CompareTo(max) > 0) return false;
                }
            }
            catch (InvalidCastException) { /* incompatible types — range check skipped */ }
            catch (FormatException)      { /* unparseable bound — range check skipped */  }
        }

        return true;
    }
}
