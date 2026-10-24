---
name: Bug Report
about: Report a bug in a BlazorComponentLibrary component or service
title: '[BUG] '
labels: bug
assignees: ''
---

## Description
A clear and concise description of the bug.

## Steps to Reproduce
1. Step one
2. Step two
3. Step three

## Expected Behavior
What you expected to happen.

## Actual Behavior
What actually happened. Include exception messages, stack traces, or console output if any.

## Minimal Razor Snippet
A minimal `.razor` example that reproduces the problem. Remove anything not needed to show the bug.

```razor
<Modal Title="Example" OnClose="HandleClose">
    Hello
</Modal>

@code {
    private void HandleClose() { }
}
```

## Environment
- BlazorComponentLibrary version (NuGet package): [e.g., 1.2.0]
- .NET SDK / runtime version: [e.g., 10.0.100]
- Blazor hosting model: [ ] Blazor Server  [ ] Blazor WebAssembly  [ ] Blazor Web App (interactive mode: Server / WebAssembly / Auto)
- Browser (if relevant): [e.g., Chrome 130 on Windows 11]
- Operating system: [e.g., Windows 11, Ubuntu 24.04]

## Additional Context
Add any other context, logs, or screenshots.
