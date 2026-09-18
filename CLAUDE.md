# CLAUDE.md

Reusable Blazor UI component library (Razor class library, `net10.0`, NuGet package `BlazorComponentLibrary`) with xUnit/bUnit tests and BenchmarkDotNet benchmarks.

## Build

```bash
dotnet restore
dotnet build                          # solution: BlazorComponentLibrary.slnx
dotnet build --configuration Release
dotnet pack -c Release                # produces the NuGet package
```

- Requires .NET 10 SDK (`Dockerfile` uses `sdk:10.0-alpine`). CI workflows in `.github/workflows/` still pin `9.0.x` - out of date relative to the csproj.
- `GenerateDocumentationFile` is on: every public member needs XML docs or the build warns.

## Tests

```bash
dotnet test                                                   # all tests
dotnet test BlazorComponentLibrary.Tests                      # library tests only
dotnet test --filter "FullyQualifiedName~ModalTests"          # single class
```

- Stack: xUnit 2.9, bUnit 1.36 (`TestContext`), FluentAssertions 6, Moq.
- Tests see `internal` members via `InternalsVisibleTo` (Tests and Benchmarks).
- Benchmarks: `dotnet run -c Release --project Benchmarks/BenchmarkRunner.csproj`.

## Lint / format

No analyzers or format gate configured. Follow `.editorconfig`: spaces, LF, 4-space indent for `.cs`/`.razor`, 2-space for csproj/json/yml, final newline.

```bash
dotnet format --verify-no-changes
```

## Key directories

| Path | Purpose |
|------|---------|
| `BlazorComponentLibrary.csproj` | The shipped library (root of repo is the project). `<Compile Remove>` excludes Tests/, Benchmarks/, examples/. |
| `Components/<Name>/` | One folder per component: `X.razor`, `X.razor.cs` (code-behind), `IX.cs` (interface), optional `X.razor.css`, `XExtensions.cs`, `XJsonExtensions.cs`, `XValidation.cs`. |
| `Services/` | DI state holders: `IThemeService`/`ThemeService`, `IToastService`/`ToastService`. |
| `Extensions/` | `ServiceCollectionExtensions.AddBlazorComponentLibrary()` (entry point for consumers), `BlazorComponentLibraryOptions`. |
| `Exceptions/` | Hierarchy rooted at `BlazorComponentLibraryException`. |
| `wwwroot/js/` | JS interop (`blazor-chart-interop.js`). |
| `_Imports.razor` | Global `@using` for all component namespaces. |
| `BlazorComponentLibrary.Tests/` | xUnit tests, one file per component/service (`ModalTests.cs`, `ToastServiceTests.cs`). |
| `Benchmarks/` | BenchmarkDotNet runner. |
| `docs/` | Per-component docs, `ARCHITECTURE.md`, migration guides, `theming.md`. |
| `examples/` | Usage samples (excluded from compile). |

Components: Accordion, Breadcrumbs, Chart, DataTable, DragDropList, Form, Modal, Pagination, ProgressBar, Skeleton, Spinner, Tabs, ThemeSwitcher, Toast.

## Conventions

- Namespace per component: `BlazorComponentLibrary.Components.<Name>`; file-scoped namespaces, `using` directives placed after the namespace line.
- `Nullable` and `ImplicitUsings` enabled; `LangVersion latest`.
- Component classes are `sealed partial class X : ComponentBase, IX` and implement `IDisposable`/`IAsyncDisposable` when holding JS refs.
- Public enums for variants live next to the component (e.g. `ModalSize` in `Modal.razor.cs`).
- Invalid parameters throw a typed exception from `Exceptions/` (e.g. `ModalException`), validated in `XValidation.cs`.
- Tests: `public sealed class XTests : TestContext`, `[Fact]` methods named `Scenario_ExpectedResult`, Arrange/Act/Assert comments, XML summaries on test methods.
- New component checklist (from CONTRIBUTING.md): code-behind `.razor.cs`, interface, docs in `docs/`, entry in README component list, `@using` in `_Imports.razor`.
- Commit messages: conventional prefixes (`feat:`, `fix:`, `docs:`, `chore:`).
- Never commit `bin/`, `obj/`, `build/`, `.aider*`.
