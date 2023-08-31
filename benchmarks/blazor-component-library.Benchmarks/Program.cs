// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BenchmarkDotNet.Running;

// Run all benchmark classes discovered in this assembly.
// Usage:
//   dotnet run -c Release                 — runs all benchmarks
//   dotnet run -c Release -- --filter *String*   — runs a subset
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
