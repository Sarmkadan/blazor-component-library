# Contributing

Contributions are welcome! Please follow these guidelines:

## Development
- .NET 10.0 SDK required.
- Clone the repo: `git clone https://github.com/sarmkadan/blazor-component-library.git`
- Build: `dotnet build`
- Tests: `dotnet test`

## Exception Handling Convention
- Use `ArgumentNullException.ThrowIfNull(x)` for parameters that cannot be null.
- Use `ArgumentException.ThrowIfNullOrWhiteSpace(s)` for string parameters that must contain content.
- Use `ArgumentOutOfRangeException` for numeric or enum parameters that fall outside valid ranges.
- Always document throws in XML comments using `<exception cref="...">Thrown when ...</exception>`.
- Avoid generic `Exception` or `InvalidOperationException` for invalid caller arguments; reserve those for internal state or framework-level issues.

## Adding a New Component
- Use the `Bcl` prefix.
- Add code-behind `.razor.cs` and documentation.
- Update `README.md` list.

## Pull Requests
- Create a branch: `feature/my-component`
- Submit a PR with a description of changes.
- Ensure `dotnet build` succeeds.

## Reporting Issues
- Open a GitHub issue with steps to reproduce and environment details.

## License
Contributions are licensed under the MIT License.
