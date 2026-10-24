# Contributing

Contributions are welcome. This guide covers setup, conventions, and what a PR needs before review.

## Prerequisites

- **.NET 10 SDK** (`net10.0`). There is no `global.json`, so any 10.0.x SDK works. Check with `dotnet --version`.
- **Git**.
- **Docker** (optional). The `Dockerfile` builds on `mcr.microsoft.com/dotnet/sdk:10.0-alpine`.

## Cloning

```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
cd blazor-component-library
```

## Build and test

```bash
dotnet restore
dotnet build                          # or: dotnet build --configuration Release
dotnet test                           # all tests (xUnit + bUnit)
dotnet test --filter "FullyQualifiedName~ModalTests"   # single test class
```

Benchmarks (BenchmarkDotNet) live in a separate project and are not run by `dotnet test`:

```bash
dotnet run -c Release --project Benchmarks/BenchmarkRunner.csproj
```

The solution file is `BlazorComponentLibrary.slnx`, which includes the library, the tests, and the benchmarks.

## Sample project

There is no runnable demo app. `examples/` holds three `.cs` usage snippets (`BasicUsage.cs`, `AdvancedUsage.cs`, `IntegrationExample.cs`). They are excluded from compilation, so treat them as reference code. The README has usage examples.

## Coding conventions

- Formatting follows [`.editorconfig`](.editorconfig): UTF-8, LF line endings, spaces, trimmed trailing whitespace, final newline. Use 4 spaces for `.cs`/`.razor` and 2 spaces for `.csproj`/`.json`/`.yml`/`.xml`.
- `dotnet format --verify-no-changes` is available, but no CI step enforces it.
- `Nullable` and `ImplicitUsings` are enabled. Use file-scoped namespaces (`namespace BlazorComponentLibrary.Components.<Name>;`) and put `using` directives after the namespace line.
- Component classes are `sealed partial class X : ComponentBase, IX`. Implement `IDisposable` or `IAsyncDisposable` when the component holds JS references.
- Public enums for variants live next to their component (for example, `ModalSize` in `Modal.razor.cs`).
- The project sets `GenerateDocumentationFile`, so every public member needs XML docs.

### Exception handling

- Use `ArgumentNullException.ThrowIfNull(x)` for parameters that cannot be null.
- Use `ArgumentException.ThrowIfNullOrWhiteSpace(s)` for string parameters that must have content.
- Use `ArgumentOutOfRangeException` for numeric or enum parameters outside their valid range.
- Invalid component parameters throw a typed exception from `Exceptions/` (for example, `ModalException`), validated in `XValidation.cs`.
- Document throws in XML comments with `<exception cref="...">Thrown when ...</exception>`.
- Don't throw generic `Exception` or `InvalidOperationException` for bad caller input. Reserve them for internal state and framework-level failures.

### Tests

- One test file per component or service, named `XTests.cs`.
- Test classes are `public sealed class XTests : TestContext` (bUnit).
- Test methods are named `Scenario_ExpectedResult`, use Arrange/Act/Assert comments, and carry an XML `<summary>`.
- Tests can see `internal` members through `InternalsVisibleTo`.

## Adding a new component

Follow the layout in [`CLAUDE.md`](CLAUDE.md) and the `Components/` folders:

- `Components/<Name>/` with `<Name>.razor`, `<Name>.razor.cs` (code-behind), and `I<Name>.cs` (interface). Add `<Name>.razor.css`, extension classes, or `<Name>Validation.cs` only when needed.
- Add `@using` for the namespace in `_Imports.razor`.
- Add documentation under `docs/`.
- Add the component to the component list in `README.md`.
- Add tests in `BlazorComponentLibrary.Tests/`.

## Branch naming

Create a branch from `main` with a type prefix that matches the commit type:

- `feat/<short-description>`, e.g. `feat/toast-progress-bar`
- `fix/<short-description>`
- `docs/<short-description>`
- `ci/<short-description>`
- `chore/<short-description>`

Use lowercase words separated by hyphens.

## Commit messages

Use [Conventional Commits](https://www.conventionalcommits.org/): `type: short summary`, with `type` one of `feat`, `fix`, `docs`, `chore`, `ci`, `test`, or `refactor`.

- Use the imperative mood and lowercase, with no trailing period: `fix: standardize exception handling in DataTable`.
- Keep the subject short. Use the body to explain why, if it isn't obvious.
- Never commit `bin/`, `obj/`, `build/`, or `.aider*` files.

## Pull requests

Before requesting review, check that:

- [ ] The branch is based on `main` and named per [Branch naming](#branch-naming).
- [ ] `dotnet build` succeeds with 0 errors.
- [ ] New behavior has tests, and `dotnet test` passes locally.
- [ ] New or changed public members have XML docs.
- [ ] New or changed components are reflected in `docs/` and `README.md`.
- [ ] CI is green. The `CI` and `Build & Test` workflows run on pull requests to `main`.
- [ ] The PR description explains what changed and why.

## Reporting issues

Open a GitHub issue with steps to reproduce, the expected and actual behavior, and your environment (.NET SDK version, OS, and browser if relevant).

## License

Contributions are licensed under the MIT License.
