# NullSafeComparerExtensions

Extension methods that provide null-safe comparison semantics for LINQ operations, sorting, and filtering. These methods wrap the underlying `NullSafeComparer` to handle null values consistently across collections, ensuring predictable ordering where nulls are always sorted last.

## API

### `NullSafeComparerExtensions.OrderByNullSafe<TSource, TKey>`

Sorts the elements of a sequence in ascending order by the specified key using null-safe comparison.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`.
  - `TKey` – The type of the key returned by `keySelector`.

- **Parameters:**
  - `source` – A sequence of values to order.
  - `keySelector` – A function to extract a key from an element.

- **Returns:** An `IOrderedEnumerable<TSource>` whose elements are sorted in ascending order according to a key.

- **Exceptions:** Throws `ArgumentNullException` if `source` or `keySelector` is null.

### `NullSafeComparerExtensions.OrderByDescendingNullSafe<TSource, TKey>`

Sorts the elements of a sequence in descending order by the specified key using null-safe comparison.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`.
  - `TKey` – The type of the key returned by `keySelector`.

- **Parameters:**
  - `source` – A sequence of values to order.
  - `keySelector` – A function to extract a key from an element.

- **Returns:** An `IOrderedEnumerable<TSource>` whose elements are sorted in descending order according to a key.

- **Exceptions:** Throws `ArgumentNullException` if `source` or `keySelector` is null.

### `NullSafeComparerExtensions.Min<TSource>`

Returns the minimum value in a sequence using null-safe comparison.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`; constrained to `IComparable<TSource>`.

- **Parameters:**
  - `source` – A sequence of values to determine the minimum of.

- **Returns:** The minimum value in the sequence, or `default` if the sequence is empty.

- **Exceptions:** Throws `ArgumentNullException` if `source` is null.

### `NullSafeComparerExtensions.Max<TSource>`

Returns the maximum value in a sequence using null-safe comparison.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`; constrained to `IComparable<TSource>`.

- **Parameters:**
  - `source` – A sequence of values to determine the maximum of.

- **Returns:** The maximum value in the sequence, or `default` if the sequence is empty.

- **Exceptions:** Throws `ArgumentNullException` if `source` is null.

### `NullSafeComparerExtensions.SortBy<TSource, TKey>`

Returns a new sequence sorted by the specified key selector in the specified direction using null-safe comparison.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`.
  - `TKey` – The type of the key returned by `keySelector`; constrained to `notnull`.

- **Parameters:**
  - `source` – A sequence of values to order.
  - `keySelector` – A function to extract a key from an element.
  - `direction` – The sort direction (ascending or descending); defaults to `SortDirection.Ascending`.

- **Returns:** A new sequence sorted according to the specified direction.

- **Exceptions:** Throws `ArgumentNullException` if `source` or `keySelector` is null.

### `NullSafeComparerExtensions.WhereNotNull<TSource>` (reference type overload)

Returns a sequence with null values filtered out, using null-safe comparison semantics. This overload works with reference types.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`.

- **Parameters:**
  - `source` – A sequence of values to filter.

- **Returns:** A new sequence containing only non-null elements.

- **Exceptions:** Throws `ArgumentNullException` if `source` is null.

### `NullSafeComparerExtensions.WhereNotNull<TSource>` (nullable value type overload)

Returns a sequence with null values filtered out, using null-safe comparison semantics. This overload works with nullable value types.

- **Type parameters:**
  - `TSource` – The type of the elements of `source`; constrained to `struct`.

- **Parameters:**
  - `source` – A sequence of nullable values to filter.

- **Returns:** A new sequence containing only non-null values.

- **Exceptions:** Throws `ArgumentNullException` if `source` is null.

## Usage

### Example 1: Ordering a collection with potential null values

```csharp
using BlazorComponentLibrary.Components.DataTable;

var items = new List<string?> { "Charlie", null, "Alice", "Bob", null };

// Ascending order (nulls last)
var sortedAsc = items.OrderByNullSafe(x => x).ToList();
// Result: ["Alice", "Bob", "Charlie", null, null]

// Descending order (nulls last)
var sortedDesc = items.OrderByDescendingNullSafe(x => x).ToList();
// Result: ["Charlie", "Bob", "Alice", null, null]
```

### Example 2: Finding min/max with null-safe comparison

```csharp
using BlazorComponentLibrary.Components.DataTable;

var numbers = new List<int?> { 5, null, 2, 8, null, 1 };

var min = numbers.Min();  // Returns 1
var max = numbers.Max();  // Returns 8

var empty = new List<int?>();
var minEmpty = empty.Min();  // Returns null
var maxEmpty = empty.Max();  // Returns null
```

## Notes

- **Null handling:** All methods treat `null` as greater than any non-null value, placing nulls at the end of sorted sequences.

- **Thread safety:** The extension methods are stateless and thread-safe; they do not maintain any mutable state between invocations.

- **Empty sequences:** Methods that return single values (`Min`, `Max`) return `default` (typically `null`) for empty sequences rather than throwing.

- **Performance:** Sorting operations (`OrderByNullSafe`, `OrderByDescendingNullSafe`, `SortBy`) use the underlying `NullSafeComparer` which performs O(n log n) comparisons. For large collections, consider materializing the result.

- **Constraints:** The `Min` and `Max` methods require `TSource` to implement `IComparable<TSource>` to enable comparison.

- **SortDirection:** The `SortBy` method accepts a `SortDirection` enum value (`Ascending` or `Descending`) to control sort order.