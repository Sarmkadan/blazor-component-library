# Skeleton

A lightweight placeholder component that renders animated or static skeleton screens to indicate loading states for content regions. It supports configurable shapes, dimensions, and line counts to approximate the layout of pending content.

## API

### Type
**Property:** `public SkeletonType Type { get; set; }`

Specifies the semantic type of the skeleton, which influences default styling and accessibility semantics. Common values include `Text`, `Circular`, `Rectangular`, and `Image`.

**Throws:** `ArgumentOutOfRangeException` if set to an undefined `SkeletonType` value.

### Shape
**Property:** `public SkeletonShape Shape { get; set; }`

Defines the geometric shape of the skeleton element. Values typically include `Circle`, `Square`, `Rounded`, and `Line`. When `Type` is `Image`, `Shape` defaults to `Rounded`; when `Type` is `Circular`, `Shape` is forced to `Circle`.

**Throws:** `ArgumentOutOfRangeException` if set to an undefined `SkeletonShape` value.

### Width
**Property:** `public string Width { get; set; }`

Sets the CSS width of the skeleton container. Accepts any valid CSS length value (e.g., `"100%"`, `"200px"`, `"auto"`). Defaults to `"100%"`.

**Throws:** `ArgumentException` if set to `null`, empty, or whitespace.

### Height
**Property:** `public string Height { get; set; }`

Sets the CSS height of the skeleton container. Accepts any valid CSS length value. Defaults vary by `Type` and `Shape` (e.g., `"1rem"` for text lines, `"48px"` for circular avatars).

**Throws:** `ArgumentException` if set to `null`, empty, or whitespace.

### Lines
**Property:** `public int Lines { get; set; }`

Number of text lines to render when `Type` is `Text`. Each line renders as a separate skeleton element with varying widths to simulate natural text raggedness. Must be greater than zero.

**Throws:** `ArgumentOutOfRangeException` if set to a value less than 1.

### Animated
**Property:** `public bool Animated { get; set; }`

Enables or disables the shimmer animation effect. When `true`, a CSS keyframe animation applies a moving gradient across the skeleton. When `false`, a static background color is used. Defaults to `true`.

## Usage

### Basic text skeleton with multiple lines
```csharp
<Skeleton Type="SkeletonType.Text" 
          Lines="3" 
          Width="100%" 
          Height="auto" 
          Animated="true" />
```

### Circular avatar placeholder with fixed dimensions
```csharp
<Skeleton Type="SkeletonType.Circular" 
          Shape="SkeletonShape.Circle" 
          Width="48px" 
          Height="48px" 
          Animated="false" />
```

## Notes

- The component renders a single root `<div>` with inline styles for `width` and `height`. Child elements are generated based on `Type` and `Lines`.
- `Width` and `Height` values are passed directly to the `style` attribute without validation beyond null/whitespace checks; invalid CSS values will be ignored by the browser.
- When `Lines` > 1 and `Type` is not `Text`, the `Lines` property is ignored.
- The shimmer animation uses `prefers-reduced-motion` media query; users with reduced motion preferences will see static skeletons regardless of `Animated` value.
- This component is stateless and thread-safe. All properties are simple value types or immutable strings; no shared mutable state exists. Safe for concurrent rendering across multiple component instances.
