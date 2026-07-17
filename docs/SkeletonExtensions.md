# SkeletonExtensions

Provides a fluent interface for configuring skeleton loading placeholders in Blazor applications. These extensions simplify the creation of animated placeholder elements that mimic the structure and layout of content while data is loading.

## API

### `Skeleton.AsText()`
Creates a skeleton placeholder styled as a text block. The placeholder will render as a series of horizontal lines to represent text content.

- **Parameters**: None
- **Return value**: `Skeleton` – A configured skeleton instance.
- **Throws**: No exceptions.

---

### `Skeleton.AsCircle()`
Creates a skeleton placeholder styled as a circular avatar or icon placeholder.

- **Parameters**: None
- **Return value**: `Skeleton` – A configured skeleton instance.
- **Throws**: No exceptions.

---
### `Skeleton.AsRectangle()`
Creates a skeleton placeholder styled as a rectangular block, suitable for images or generic content areas.

- **Parameters**: None
- **Return value**: `Skeleton` – A configured skeleton instance.
- **Throws**: No exceptions.

---
### `Skeleton.WithWidth(width)`
Sets the width of the skeleton placeholder.

- **Parameters**:
  - `width` (`string`) – A valid CSS width value (e.g., `"100px"`, `"50%"`).
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: `ArgumentNullException` if `width` is `null` or whitespace.

---
### `Skeleton.WithHeight(height)`
Sets the height of the skeleton placeholder.

- **Parameters**:
  - `height` (`string`) – A valid CSS height value (e.g., `"100px"`, `"auto"`).
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: `ArgumentNullException` if `height` is `null` or whitespace.

---
### `Skeleton.WithLines(count)`
Sets the number of lines to render for text-style skeletons.

- **Parameters**:
  - `count` (`int`) – The number of lines to display. Must be a positive integer.
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: `ArgumentOutOfRangeException` if `count` is less than 1.

---
### `Skeleton.WithAnimation(animation)`
Configures the animation type for the skeleton.

- **Parameters**:
  - `animation` (`SkeletonAnimation`) – An enum value specifying the animation (e.g., `Pulse`, `Wave`, `None`).
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

---
### `Skeleton.Animated()`
Enables animation on the skeleton placeholder using the default animation style.

- **Parameters**: None
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

---
### `Skeleton.Static()`
Disables animation on the skeleton placeholder.

- **Parameters**: None
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

---
### `Skeleton.AsAvatar()`
Configures the skeleton as an avatar-style placeholder, typically circular and centered.

- **Parameters**: None
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

---
### `Skeleton.AsButton()`
Configures the skeleton as a button-style placeholder, with a rectangular shape and text-like appearance.

- **Parameters**: None
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

---
### `Skeleton.AsCard()`
Configures the skeleton as a card-style placeholder, typically a rectangular block with padding and multiple internal lines.

- **Parameters**: None
- **Return value**: `Skeleton` – The same skeleton instance for method chaining.
- **Throws**: No exceptions.

## Usage
