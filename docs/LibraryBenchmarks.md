# LibraryBenchmarks

The `LibraryBenchmarks` type provides a collection of benchmark harnesses for measuring the performance of common operations in the Blazor component library, such as sorting algorithms, data‑table manipulations, and drag‑drop list reordering. It exposes methods that generate test data, execute the operations, and return the results for timing or validation, along with simple properties for tagging benchmark instances.

## API

### Setup
```csharp
public void Setup()
```
Prepares internal state required by the benchmark methods. This method must be called before invoking any of the data‑generating or operation methods. It accepts no parameters and returns void. Throws `InvalidOperationException` if called more than once or if the underlying resources cannot be initialized.

### SortWithNullSafeComparer
```csharp
public List<string?> SortWithNullSafeComparer()
```
Generates a list of nullable strings and sorts it using a null‑safe comparer. Returns the sorted list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### SortWithNullChecks
```csharp
public List<string?> SortWithNullChecks()
```
Generates a list of nullable strings and sorts it using explicit null checks before comparison. Returns the sorted list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### SortComplexDataById
```csharp
public List<DataItem> SortComplexDataById()
```
Creates a collection of `DataItem` instances and sorts them by the `Id` property. Returns the sorted list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### SortComplexDataByName
```csharp
public List<DataItem> SortComplexDataByName()
```
Creates a collection of `DataItem` instances and sorts them by the `Name` property. Returns the sorted list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### SortComplexDataByStatus
```csharp
public List<DataItem> SortComplexDataByStatus()
```
Creates a collection of `DataItem` instances and sorts them by the `Status` property. Returns the sorted list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### DataTableSetData
```csharp
public void DataTableSetData()
```
Populates an internal data table with a predefined dataset used by the subsequent sorting benchmarks. No parameters, returns void. Throws `InvalidOperationException` if `Setup` has not been invoked.

### DataTableSortById
```csharp
public void DataTableSortById()
```
Sorts the internal data table by the `Id` column. No parameters, returns void. Throws `InvalidOperationException` if `Setup` or `DataTableSetData` has not been invoked.

### DataTableSortByName
```csharp
public void DataTableSortByName()
```
Sorts the internal data table by the `Name` column. No parameters, returns void. Throws `InvalidOperationException` if `Setup` or `DataTableSetData` has not been invoked.

### DataTableSortByStatus
```csharp
public void DataTableSortByStatus()
```
Sorts the internal data table by the `Status` column. No parameters, returns void. Throws `InvalidOperationException` if `Setup` or `DataTableSetData` has not been invoked.

### DragDropListReorderSmall
```csharp
public List<string> DragDropListReorderSmall()
```
Generates a small list of strings and simulates a drag‑drop reorder operation, returning the resulting list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### DragDropListReorderLarge
```csharp
public List<string> DragDropListReorderLarge()
```
Generates a large list of strings and simulates a drag‑drop reorder operation, returning the resulting list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### DragDropListReorderFirstToLast
```csharp
public List<string> DragDropListReorderFirstToLast()
```
Generates a list of strings and moves the first element to the last position via a drag‑drop simulation, returning the reordered list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### DragDropListReorderLastToFirst
```csharp
public List<string> DragDropListReorderLastToFirst()
```
Generates a list of strings and moves the last element to the first position via a drag‑drop simulation, returning the reordered list. No parameters. Throws `InvalidOperationException` if `Setup` has not been invoked.

### Id
```csharp
public int Id
```
Gets or sets the identifier for the benchmark instance. No parameters. Returns an `int`. Does not throw under normal use.

### Name
```csharp
public string? Name
```
Gets or sets an optional name associated with the benchmark instance. No parameters. Returns a nullable `string`. Does not throw under normal use.

### Email
```csharp
public string? Email
```
Gets or sets an optional email address associated with the benchmark instance. No parameters. Returns a nullable `string`. Does not throw under normal use.

### Status
```csharp
public string? Status
```
Gets or sets an optional status value associated with the benchmark instance. No parameters. Returns a nullable `string`. Does not throw under normal use.

### CreatedDate
```csharp
public DateTime CreatedDate
```
Gets the date and time when the benchmark instance was created. No parameters. Returns a `DateTime`. Does not throw under normal use.

## Usage

### Example 1: Basic sorting benchmark
```csharp
using BlazorComponentLibrary.Benchmarks;

var bench = new LibraryBenchmarks();
bench.Setup();                                 // prepare internal state

var sortedById = bench.SortComplexDataById();  // get list sorted by Id
var sortedByName = bench.SortComplexDataByName(); // get list sorted by Name

// Use the results for timing or validation
Console.WriteLine($"First item by Id: {sortedById[0].Id}");
Console.WriteLine($"First item by Name: {sortedByName[0].Name}");
```

### Example 2: Drag‑drop reorder benchmark
```csharp
using BlazorComponentLibrary.Benchmarks;

var bench = new LibraryBenchmarks();
bench.Setup();

var smallList = bench.DragDropListReorderSmall();
var largeList = bench.DragDropListReorderLarge();

// Verify that reordering produced a list of the same size
System.Diagnostics.Debug.Assert(smallList.Count == bench.DragDropListReorderSmall().Count);
System.Diagnostics.Debug.Assert(largeList.Count == bench.DragDropListReorderLarge().Count);
```

## Notes

- All data‑generating methods (`Sort*`, `DataTable*`, `DragDropList*`) depend on state initialized by `Setup`. Calling them before `Setup` results in an `InvalidOperationException`.
- The class is **not thread‑safe**. Concurrent calls to `Setup` or any of the benchmark methods from multiple threads may lead to race conditions, corrupted internal state, or unexpected exceptions. If parallel benchmarking is required, instantiate a separate `LibraryBenchmarks` object per thread.
- The nullable string‑returning methods (`SortWithNullSafeComparer`, `SortWithNullChecks`) may produce lists that contain `null` elements; callers should handle nulls appropriately when consuming the results.
- The `Id`, `Name`, `Email`, `Status`, and `CreatedDate` properties are simple mutable fields (except `CreatedDate`, which is set once at construction). They do not affect the benchmark logic and are provided for contextual tagging of benchmark runs.
- Exception messages are not part of the public contract; rely only on the exception type (`InvalidOperationException`) for flow control.
