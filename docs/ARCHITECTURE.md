# Architecture

This document describes how the library is actually structured, why it is structured
that way, and where the sharp edges are. It reflects the code as of v1.x.

## Overview

BlazorComponentLibrary is a single Razor class library (`Microsoft.NET.Sdk.Razor`,
`net10.0`) with two supporting projects that live in the same repo but are excluded
from the package build:

```
BlazorComponentLibrary.csproj      the shipped library
├── Components/                    one folder per component (razor + code-behind + interface + scoped css)
│   ├── Chart/
│   ├── DataTable/
│   ├── DragDropList/
│   ├── Form/
│   ├── Modal/
│   ├── Skeleton/
│   ├── ThemeSwitcher/
│   └── Toast/                     ToastContainer (renders IToastService state)
├── Services/                      DI-registered state holders: ThemeService, ToastService
├── Extensions/                    ServiceCollectionExtensions.AddBlazorComponentLibrary()
├── Exceptions/                    library exception hierarchy rooted at BlazorComponentLibraryException
├── BlazorComponentLibrary.Tests/  xUnit tests (sees internals via InternalsVisibleTo)
└── Benchmarks/                    BenchmarkDotNet project (sorting, drag-drop reorder, DataTable ops)
```

The main csproj uses `<Compile Remove>` to keep `Tests/`, `Benchmarks/` and
`examples/` out of the package even though they are nested inside the library
directory. That is a deliberate trade-off: a flat repo layout with everything under
one root is easier to navigate for a small library, at the cost of slightly unusual
csproj plumbing. `InternalsVisibleTo` is granted to both Tests and Benchmarks so
component internals (e.g. `DragDropList._draggingIndex`) can be exercised without
widening the public API.

## Component pattern

Every component follows the same four-file convention:

| File | Role |
|------|------|
| `X.razor` | Markup only |
| `X.razor.cs` | `sealed partial class`, parameters and logic |
| `IX.cs` | Public interface the component implements |
| `X.razor.css` | Scoped CSS (Blazor CSS isolation) |

Rationale for the per-component interface (`IDataTable<TItem>`, `IModal`, ...):
it pins down the public contract explicitly and lets tests and benchmarks program
against the surface rather than the concrete class. Trade-off: it is extra
ceremony for components that will realistically never have a second
implementation; we accept that for API-stability discipline in a published
package.

Components that mutate their own state outside the normal parameter flow
(`SetData`, `SetModel`, `SortBy`) wrap `StateHasChanged()` in a
`NotifyStateChanged()` helper that swallows `InvalidOperationException`. This is
what makes the classes usable in plain unit tests and BenchmarkDotNet runs where
no renderer is attached. It also means a genuinely wrong-threaded
`StateHasChanged` call would be silenced - a known limitation, accepted because
the library targets simple call patterns.

## Services and data flow

Two services carry cross-component state. Both are registered scoped (per-circuit
on Blazor Server, effectively singleton on WebAssembly) via
`AddBlazorComponentLibrary()`:

- **`IToastService` / `ToastService`** - in-memory toast queue. `Show()` appends a
  `ToastMessage` record, raises `ToastsChanged`, and (for positive durations)
  schedules auto-dismiss on a `System.Timers.Timer`. `ToastContainer` subscribes
  to `ToastsChanged` in `OnInitialized`, re-renders via
  `InvokeAsync(StateHasChanged)` (timer callbacks fire off the sync context), and
  unsubscribes in `Dispose`. All list/timer access is under one lock because timer
  callbacks are threadpool threads. Design decision: the service owns dismissal
  timers rather than each toast component owning its own async lifetime - one
  place to dispose, no orphaned delays after `DismissAll()`.

- **`IThemeService` / `ThemeService`** - holds the current `ThemeMode`
  (Light/Dark/System), persists it in `localStorage` under `bcl-theme`, and stamps
  `data-bcl-theme` on `document.documentElement` so CSS custom properties switch
  without a reload. `SetTheme` is synchronous by contract; the JS interop push is
  deliberately fire-and-forget with faults observed inside `PushToBrowserAsync`
  (pre-rendering makes interop unavailable, and the in-memory theme stays
  authoritative). `InitializeAsync()` must be called once from the root layout's
  first `OnAfterRenderAsync`; stored values are validated with
  `Enum.TryParse` + `Enum.IsDefined` so junk in localStorage cannot smuggle in an
  undefined enum value.

Typical flow: app code injects a service → service mutates state → C# event fires
→ subscribed component calls `StateHasChanged` → render. There is no message bus,
no cascading state container - two events are enough at this size.

## JS interop

There is no bundled JS file. The few browser touches (theme attribute,
localStorage, modal focus save/restore) go through `IJSRuntime` with `eval`.
That keeps the package free of static web assets to load, but it is the weakest
part of the design: `eval` is blocked by strict CSP (`unsafe-eval`) and the
snippets are unminified strings. If a consumer report ever lands on CSP, the fix
is a small `wwwroot/bcl.js` module with named functions - the call sites are
already isolated (`ThemeService.PushToBrowserAsync`, `Modal.Show/Hide`), so the
swap is mechanical. Interpolated values are enum-derived, never user input, so
there is no injection surface today; keep it that way.

`Modal` saves `document.activeElement` into `window.__bclModalTrigger` on `Show()`
and restores focus on `Hide()` (WCAG 2.1 SC 2.4.3). One global slot means nested
modals will restore focus to the wrong trigger - known limitation, documented
here rather than solved with a stack because nested modals are not a supported
scenario.

## Error handling

All library exceptions derive from `BlazorComponentLibraryException`, with
specific types per area (`ToastServiceException`, `ThemeServiceException`,
`ModalException`, `ValidationException`, `ConfigurationException`). Low-level
exceptions are wrapped (`catch (Exception ex) when (ex is not XException)`) so
consumers can catch one base type at the app boundary. Expected environmental
failures are not exceptions: pre-render interop unavailability and blocked
localStorage are swallowed with comments explaining why.

## Rendering / performance decisions

- **DataTable**: sorting uses `NullSafeComparer` (nulls last, falls back to
  ordinal string compare for non-`IComparable`) - measurably slower than a typed
  comparer (see README benchmark table) but immune to `NullReferenceException`
  from sparse columns. `EnableVirtualization` hands windowing to Blazor's
  `Virtualize` and disables pagination; the two modes are mutually exclusive by
  design because paginating a virtualized window is meaningless.
- **DragDropList** reorders via HTML5 drag events and reports the *entire*
  reordered list through `OnOrderChanged`, so the parent persists order without
  diffing. Costs an allocation per drop (~8 KB in benchmarks), buys a trivially
  correct consumer contract.
- **Form\<TModel\>** validates with `System.ComponentModel.DataAnnotations`
  (`Validator.TryValidateObject`, `validateAllProperties: true`) on submit only -
  no per-keystroke validation, no dependency on `EditForm`/`EditContext`. Simpler
  model, but no field-level validation messages until submit.
- **Chart** currently renders data purely server-side markup (no charting JS
  library); `Annotations` (threshold lines, markers, bands) are part of the
  parameter surface. Treat it as the least mature component.

## Extension points

- **Theming**: everything visual reads `--bcl-*` CSS custom properties; override
  them in the consuming app (see [theming.md](theming.md)). No C# needed for a reskin.
- **Templates**: `DataTable` (`TableHeader`, `RowTemplate`), `DragDropList`
  (`ItemTemplate`), `Modal` (`ChildContent`, `FooterContent`), `Form`
  (`ChildContent`) all take `RenderFragment`s.
- **Service replacement**: registrations use `TryAddScoped`, so an app can
  register its own `IToastService`/`IThemeService` implementation *before*
  calling `AddBlazorComponentLibrary()` and the library will use it.
- **Events**: `IThemeService.ThemeChanged`, `IToastService.ToastsChanged` for app
  code that wants to react to library state.

## Known limitations

- `eval`-based JS interop breaks under strict CSP (see above).
- Modal focus restore does not support nesting.
- DataTable has no built-in filtering UI despite an `IsFilterable` parameter -
  the flag currently only reserves the API.
- Pagination is fixed to the first page (`Take(PageSize)`); there is no page
  navigation state yet.
- Chart has no interactive rendering backend.
- `ToastService` timers are per-toast `System.Timers.Timer` instances - fine for
  human-scale toast volumes, wasteful if abused programmatically.
