# Contributing to Blazor Component Library

Thank you for your interest in contributing! This guide covers the development workflow and conventions.

## Development Setup

### Prerequisites

- .NET 10.0 SDK
- A code editor with Razor/C# support (Visual Studio, Rider, or VS Code with C# Dev Kit)

### Build and Run

```bash
git clone https://github.com/YOUR_USERNAME/blazor-component-library.git
cd blazor-component-library
dotnet restore
dotnet build
dotnet run
```

## Adding a New Component

1. Create a new `.razor` file in `Components/` following existing naming conventions (`Bcl` prefix).
2. Add a code-behind `.razor.cs` file for component logic and parameter definitions.
3. Document all `[Parameter]` properties with XML `<summary>` comments.
4. Add CSS isolation via a `.razor.css` file alongside the component.
5. Register any required services in `Extensions/ServiceCollectionExtensions.cs`.
6. Update the README component list.

### Component Conventions

- All public parameters must have XML doc comments.
- Use `EventCallback<T>` for event parameters, not raw `Action<T>`.
- Support both bound values (`@bind-Value`) and unbound usage where applicable.
- Include `aria-*` attributes for accessibility.
- Prefer CSS custom properties over hardcoded colors/sizes for theming.

## Pull Request Process

1. Fork the repository and create a branch: `git checkout -b feature/my-component`
2. Make your changes following the conventions above.
3. Ensure `dotnet build` succeeds with zero warnings.
4. Submit a PR with a clear description of the component or change.

## Reporting Issues

Open a GitHub issue with:
- Component name (if applicable)
- Steps to reproduce
- Expected vs actual behavior
- Browser and .NET SDK version

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
