namespace BlazorComponentLibrary.Tests;

using BlazorComponentLibrary.Components.DataTable;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

/// <summary>
/// Extension methods for <see cref="DataTableColumnVisibilityTests"/> to provide grouped test execution capabilities.
/// </summary>
public static class DataTableColumnVisibilityTestsExtensions
{
    /// <summary>
    /// Executes all column visibility property tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteHiddenColumnsTests(this DataTableColumnVisibilityTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        tests.HiddenColumns_ShouldBeSetAndRetrieved();
        tests.HiddenColumns_SetToNull_ShouldCreateEmptySet();
    }

    /// <summary>
    /// Executes all column toggling tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteToggleColumnTests(this DataTableColumnVisibilityTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        tests.ToggleColumn_ShouldAddColumnWhenNotPresent();
        tests.ToggleColumn_ShouldRemoveColumnWhenPresent();
        tests.ToggleColumn_ShouldToggleVisibility();

        // Test invalid column names
        var invalidNames = new List<string?> { null, "", "   " };
        foreach (var invalidName in invalidNames)
        {
            tests.ToggleColumn_ShouldThrowForInvalidColumnName(invalidName);
        }

        tests.ToggleColumn_ShouldNotifyStateChanged();
    }

    /// <summary>
    /// Executes all interface implementation tests as a group.
    /// </summary>
    /// <param name="tests">The test instance to execute tests on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void ExecuteInterfaceTests(this DataTableColumnVisibilityTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);

        tests.IDataTable_ToggleColumn_ShouldBeImplemented();
        tests.IDataTable_HiddenColumns_ShouldBeImplemented();
    }
}