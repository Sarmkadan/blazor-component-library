// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Core contract for a server-side virtualized data grid engine.
/// <para>
/// Implementations provide windowed access to potentially large data sets together with
/// multi-column sorting, compound column filtering, full-text search, transactional inline
/// editing with undo support, and optional distributed result caching.
/// </para>
/// <para>
/// The engine is intentionally data-source agnostic: it delegates raw row access to
/// <see cref="Repositories.IDataRepository"/> and persistence of cell edits to
/// <see cref="IGridEditHandler"/>, making it straightforward to swap either component
/// without touching the query or undo logic.
/// </para>
/// </summary>
public interface IVirtualizedGridService
{
    /// <summary>
    /// Executes a virtual-window query against the specified table, applying all active
    /// filters, sorts, and full-text search before slicing the result to the requested window.
    /// </summary>
    /// <param name="tableId">Identifier of the source data table.</param>
    /// <param name="request">
    ///   Window parameters: zero-based start index, row count, active
    ///   <see cref="GridFilterDescriptor"/> list, <see cref="GridSortDescriptor"/> list,
    ///   and optional search term.
    /// </param>
    /// <param name="cancellationToken">Token to observe for request cancellation.</param>
    /// <returns>
    ///   A <see cref="GridVirtualResult{DataTableRow}"/> containing the requested row window,
    ///   unfiltered total count, filtered total count, and query metadata.
    /// </returns>
    Task<GridVirtualResult<DataTableRow>> QueryAsync(
        int tableId,
        GridVirtualRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies a single inline cell edit, validates it against the column's constraints,
    /// persists the change, and records the previous value in the undo stack.
    /// </summary>
    /// <param name="tableId">Table that owns the target row.</param>
    /// <param name="edit">Edit descriptor: row ID, column key, new value, and optional original value for concurrency checking.</param>
    /// <param name="cancellationToken">Token to observe for request cancellation.</param>
    /// <returns>
    ///   A <see cref="GridEditResult"/> indicating whether the edit was committed, the
    ///   applied value (which may differ from the requested value if coerced), and an
    ///   error message on failure.
    /// </returns>
    Task<GridEditResult> ApplyEditAsync(
        int tableId,
        GridEditRequest edit,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies multiple cell edits as a single atomic unit: all edits are validated before
    /// any are committed, so a single validation failure prevents any writes from occurring.
    /// Execution halts at the first commit failure if validation passes.
    /// </summary>
    /// <param name="tableId">Table that owns the target rows.</param>
    /// <param name="edits">Ordered collection of edit descriptors to commit.</param>
    /// <param name="cancellationToken">Token to observe for request cancellation.</param>
    /// <returns>
    ///   A list of <see cref="GridEditResult"/> values in the same order as the input,
    ///   truncated at the first failure.
    /// </returns>
    Task<IReadOnlyList<GridEditResult>> ApplyBatchEditsAsync(
        int tableId,
        IEnumerable<GridEditRequest> edits,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a proposed edit against the column's constraints without writing any change.
    /// Useful for real-time editor feedback before the user presses Commit.
    /// </summary>
    /// <param name="tableId">Table whose column definitions supply the validation rules.</param>
    /// <param name="edit">Edit descriptor to validate.</param>
    /// <returns>
    ///   <c>true</c> when the proposed value satisfies all column constraints;
    ///   <c>false</c> otherwise.
    /// </returns>
    Task<bool> ValidateEditAsync(int tableId, GridEditRequest edit);

    /// <summary>
    /// Returns the per-table edit history in reverse-chronological order (newest entry first).
    /// </summary>
    /// <param name="tableId">Target table identifier.</param>
    /// <param name="rowId">
    ///   When supplied, restricts the result to edits on that specific row.
    ///   Pass <c>null</c> to return the full table history.
    /// </param>
    /// <returns>Ordered list of <see cref="GridEditHistoryEntry"/> records.</returns>
    Task<IReadOnlyList<GridEditHistoryEntry>> GetEditHistoryAsync(
        int tableId,
        int? rowId = null);

    /// <summary>
    /// Reverses the most recently committed edit for the given table by writing the
    /// captured <see cref="GridEditHistoryEntry.PreviousValue"/> back to the repository.
    /// </summary>
    /// <param name="tableId">Table whose last edit should be reversed.</param>
    /// <returns>
    ///   <c>true</c> if an edit was successfully undone;
    ///   <c>false</c> if the undo stack was empty or the reverse write failed.
    /// </returns>
    Task<bool> UndoLastEditAsync(int tableId);

    /// <summary>
    /// Retrieves the effective column definitions for a table including all grid-specific
    /// overrides registered via <see cref="UpdateColumnDefinitionAsync"/>.
    /// </summary>
    /// <param name="tableId">Target table identifier.</param>
    /// <returns>Ordered list of <see cref="GridColumnDefinition"/> instances.</returns>
    Task<IReadOnlyList<GridColumnDefinition>> GetColumnDefinitionsAsync(int tableId);

    /// <summary>
    /// Persists an updated <see cref="GridColumnDefinition"/> for the specified table,
    /// matched by <see cref="DataTableColumn.Key"/>.
    /// </summary>
    /// <param name="tableId">Table whose column definition should be replaced.</param>
    /// <param name="column">Updated column definition carrying the new constraints and display overrides.</param>
    /// <returns>
    ///   <c>true</c> if the definition was located and replaced;
    ///   <c>false</c> if no column with a matching key was registered for the table.
    /// </returns>
    Task<bool> UpdateColumnDefinitionAsync(int tableId, GridColumnDefinition column);

    /// <summary>
    /// Purges all cached query windows for a table, forcing the next query to be fully
    /// recomputed from the repository.  Called automatically after every successful edit.
    /// </summary>
    /// <param name="tableId">Table whose result cache should be invalidated.</param>
    Task InvalidateCacheAsync(int tableId);
}

/// <summary>
/// Handles the validation and persistence side-effects of a single inline grid cell edit.
/// Decoupled from <see cref="IVirtualizedGridService"/> to allow pluggable edit back-ends:
/// in-memory store, relational database, remote REST API, event-sourced log, etc.
/// </summary>
public interface IGridEditHandler
{
    /// <summary>
    /// Validates a proposed edit against column constraints and, if valid, mutates the supplied
    /// <see cref="DataTableRow"/> to reflect the new value.
    /// </summary>
    /// <param name="row">The row object to mutate on success.</param>
    /// <param name="edit">The edit descriptor containing the target field and proposed value.</param>
    /// <param name="column">
    ///   Optional column definition providing validation rules (<see cref="GridColumnDefinition.ValidateValue"/>).
    ///   When <c>null</c>, no column-level validation is performed.
    /// </param>
    /// <returns>
    ///   A <see cref="GridEditResult"/> describing the outcome.
    ///   On success the result carries the value actually written (which may differ from the requested
    ///   value if the handler coerced it).
    /// </returns>
    Task<GridEditResult> HandleEditAsync(
        DataTableRow row,
        GridEditRequest edit,
        GridColumnDefinition? column);

    /// <summary>
    /// Returns <c>true</c> when the given field on the given row is currently editable,
    /// without throwing an exception.  Used by the service layer to produce a user-friendly
    /// error response rather than an unhandled exception.
    /// </summary>
    /// <param name="row">Target row.</param>
    /// <param name="field">Column key to check.</param>
    /// <param name="column">Optional column definition; when <c>null</c> defaults to editable.</param>
    bool CanEdit(DataTableRow row, string field, GridColumnDefinition? column);
}
