namespace BlazorComponentLibrary.Tests;

using Bunit;
using Xunit;
using BlazorComponentLibrary.Components.Chart;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension methods for <see cref="ChartRazorUnitTests"/> to provide grouped test execution capabilities.
/// </summary>
public static class ChartRazorUnitTestsExtensions
{
    /// <summary>
    /// Executes all chart type validation tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteChartTypeTests(this ChartRazorUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        // Test all ChartType enum values
        var chartTypes = Enum.GetValues<ChartType>();
        foreach (var chartType in chartTypes)
        {
            tests.ChartType_Set_AllEnumValues_RenderCorrectly(chartType);
        }
    }

    /// <summary>
    /// Executes all data-related tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteDataTests(this ChartRazorUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        // Test SetData with various scenarios
        tests.SetData_WithValidData_SetsDataCorrectly();
        tests.SetData_NullData_HandledGracefully();
        tests.SetData_EmptyData_HandledCorrectly();

        // Test Refresh functionality
        tests.Refresh_Invoked_DoesNotThrow();
        tests.Refresh_MultipleTimes_WorksCorrectly();
    }

    /// <summary>
    /// Executes all annotation-related tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteAnnotationTests(this ChartRazorUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        // Test annotations with collection
        tests.Annotations_Set_Collection_RendersCorrectly();
        tests.Annotations_Set_EmptyCollection_HandledCorrectly();
        tests.Annotations_Null_ThrowsArgumentNullException();
    }

    /// <summary>
    /// Executes all core property tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteCorePropertyTests(this ChartRazorUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        // Test default render
        tests.DefaultRender_HasDefaultValues();

        // Test title handling
        tests.Title_Set_VariousValues_RendersCorrectly("Test Title");
        tests.Title_NullOrWhitespace_HandledGracefully(null);
        tests.Title_NullOrWhitespace_HandledGracefully("");
        tests.Title_NullOrWhitespace_HandledGracefully(" ");

        // Test labels handling
        tests.Labels_Set_Collection_RendersCorrectly();
        tests.Labels_Set_EmptyCollection_HandledCorrectly();
        tests.Labels_Null_HandledGracefully();

        // Test colors handling
        tests.Colors_Set_Collection_RendersCorrectly();
        tests.Colors_Set_EmptyCollection_HandledCorrectly();
        tests.Colors_Null_HandledGracefully();

        // Test options handling
        tests.Options_Set_VariousObjects_RendersCorrectly();
        tests.Options_Null_HandledGracefully();
    }
}