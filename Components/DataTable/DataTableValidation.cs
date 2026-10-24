namespace BlazorComponentLibrary.Components.DataTable;

using BlazorComponentLibrary.Exceptions;

/// <summary>
/// Guards that keep a <see cref="DataTable{TItem}"/> from being handed more rows than it can render.
/// </summary>
internal static class DataTableValidation
{
    /// <summary>
    /// Ensures that <paramref name="data"/> holds no more than <paramref name="maxRows"/> items.
    /// Enumeration stops one item past the limit, so an unbounded source is never fully read.
    /// </summary>
    /// <typeparam name="TItem">The type of the row items.</typeparam>
    /// <param name="data">The data source to check.</param>
    /// <param name="maxRows">The maximum number of rows the table accepts.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
    /// <exception cref="DataTableException">Thrown when <paramref name="data"/> contains more than <paramref name="maxRows"/> items.</exception>
    public static void EnsureWithinMaxRows<TItem>(IEnumerable<TItem> data, int maxRows)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Skip(maxRows).Any())
        {
            throw new DataTableException(
                $"DataTable was given more than {maxRows} rows. Page or filter the source before binding it, " +
                "or raise MaxRows explicitly if the larger set is intended.");
        }
    }
}
