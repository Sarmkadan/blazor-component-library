namespace BlazorComponentLibrary.Tests;

using System;
using System.Collections.Generic;

/// <summary>
/// Extension methods for <see cref="SpinnerTests"/> to facilitate grouped test execution and discovery.
/// </summary>
public static class SpinnerTestsExtensions
{
    /// <summary>
    /// Executes all rendering and size-related test scenarios for the spinner component.
    /// </summary>
    /// <param name="tests">The test instance to execute scenarios on. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void RunAllRenderScenarios(this SpinnerTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.DefaultRender_HasMediumSizeAndCurrentColor();
        tests.Size_Small_RendersSmallClass();
        tests.Size_Large_RendersLargeClass();
        tests.Color_Set_RendersCorrectStyle();
    }

    /// <summary>
    /// Executes accessibility-related test scenarios for the spinner component.
    /// </summary>
    /// <param name="tests">The test instance to execute scenarios on. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void RunAccessibilityScenarios(this SpinnerTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.Label_Set_RendersAriaLabel();
    }

    /// <summary>
    /// Returns the names of all available test scenarios for the spinner component.
    /// </summary>
    /// <param name="tests">The test instance. Unused, provided for fluent API consistency.</param>
    /// <returns>An immutable list of test scenario names.</returns>
    public static IReadOnlyList<string> GetTestMethodNames(this SpinnerTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        return
        [
            nameof(SpinnerTests.DefaultRender_HasMediumSizeAndCurrentColor),
            nameof(SpinnerTests.Size_Small_RendersSmallClass),
            nameof(SpinnerTests.Size_Large_RendersLargeClass),
            nameof(SpinnerTests.Color_Set_RendersCorrectStyle),
            nameof(SpinnerTests.Label_Set_RendersAriaLabel),
            nameof(SpinnerTests.DefaultValues_AreAsDocumented)
        ];
    }
}
