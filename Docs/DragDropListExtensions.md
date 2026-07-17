# DragDropListExtensions

`DragDropListExtensions` provides a set of extension methods that operate on `DragDropList<TItem>` instances. These helpers simplify common list manipulation tasks such as moving items, reordering, and querying the list without exposing the internal implementation details of `DragDropList<TItem>`.

## API

### `public static void MoveItem<TItem>(this DragDropList<TItem> list, TItem item, int fromIndex, int toIndex)`

Moves the specified `item` from `fromIndex` to `toIndex` within the list.

* **Parameters**
  * `list` – The `DragDropList<TItem>` to operate on. Must not be `null`.
  * `item` – The item to move. Must be present in the list at `fromIndex`.
  * `fromIndex` – Zero‑based index of the item's current position. Must be within the bounds of the list.
  * `toIndex` – Zero‑based index where the item should be placed. Must be within the bounds of the list (or equal to `list.Count` to append).

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.
  * `ArgumentOutOfRangeException` – If `fromIndex` or `toIndex` is outside the valid range.
  * `InvalidOperationException` – If `item` is not found at `fromIndex`.

---

### `public static void MoveToBeginning<TItem>(this DragDropList<TItem> list, TItem item)`

Moves `item` to the first position of the list.

* **Parameters**
  * `list` – The target `DragDropList<TItem>`. Must not be `null`.
  * `item` – The item to move. Must exist in the list.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.
  * `InvalidOperationException` – If `item` is not present in the list.

---

### `public static void MoveToEnd<TItem>(this DragDropList<TItem> list, TItem item)`

Moves `item` to the last position of the list.

* **Parameters**
  * `list` – The target `DragDropList<TItem>`. Must not be `null`.
  * `item` – The item to move. Must exist in the list.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.
  * `InvalidOperationException` – If `item` is not present in the list.

---

### `public static void SwapItems<TItem>(this DragDropList<TItem> list, int index1, int index2)`

Exchanges the items at `index1` and `index2`.

* **Parameters**
  * `list` – The `DragDropList<TItem>` to modify. Must not be `null`.
  * `index1` – First index. Must be within the list bounds.
  * `index2` – Second index. Must be within the list bounds.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.
  * `ArgumentOutOfRangeException` – If either index is out of range.

---

### `public static int IndexOf<TItem>(this DragDropList<TItem> list, TItem item)`

Returns the zero‑based index of `item` or `-1` if the item is not found.

* **Parameters**
  * `list` – The source `DragDropList<TItem>`. Must not be `null`.
  * `item` – The item to locate. May be `null` if the list permits null entries.

* **Return Value**
  * Index of the first occurrence of `item`, or `-1` when absent.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.

---

### `public static bool Contains<TItem>(this DragDropList<TItem> list, TItem item)`

Indicates whether `item` exists in the list.

* **Parameters**
  * `list` – The `DragDropList<TItem>` to query. Must not be `null`.
  * `item` – The item to test for presence.

* **Return Value**
  * `true` if the list contains `item`; otherwise `false`.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.

---

### `public static int Count<TItem>(this DragDropList<TItem> list)`

Gets the number of items currently stored in the list.

* **Parameters**
  * `list` – The `DragDropList<TItem>` whose count is required. Must not be `null`.

* **Return Value**
  * The total number of items.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.

---

### `public static IReadOnlyList<TItem> AsReadOnly<TItem>(this DragDropList<TItem> list)`

Returns a read‑only wrapper around the underlying collection.

* **Parameters**
  * `list` – The source `DragDropList<TItem>`. Must not be `null`.

* **Return Value**
  * An `IReadOnlyList<TItem>` that reflects the current state of the list but does not allow modification.

* **Exceptions**
  * `ArgumentNullException` – If `list` is `null`.

## Usage

### Example 1 – Reordering items in a drag‑and‑drop list

