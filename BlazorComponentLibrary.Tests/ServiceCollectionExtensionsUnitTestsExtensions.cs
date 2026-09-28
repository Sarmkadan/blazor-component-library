using System;
using BlazorComponentLibrary.Tests;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Extension methods for <see cref="ServiceCollectionExtensionsUnitTests"/> to group test execution.
/// </summary>
public static class ServiceCollectionExtensionsUnitTestsExtensions
{
    /// <summary>
    /// Runs all tests in the <see cref="ServiceCollectionExtensionsUnitTests"/> class.
    /// </summary>
    /// <param name="tests">The test instance.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="tests"/> is <see langword="null"/>.</exception>
    public static void RunAllTests(this ServiceCollectionExtensionsUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.AddBlazorComponentLibrary_NullServices_ThrowsArgumentNullException();
        tests.AddBlazorComponentLibrary_NullConfigure_DoesNotThrow();
        tests.AddBlazorComponentLibrary_Idempotent_DoesNotDuplicateRegistrations();
        tests.AddBlazorComponentLibrary_ResolvesServices();
    }

    /// <summary>
    /// Runs registration-related tests in the <see cref="ServiceCollectionExtensionsUnitTests"/> class.
    /// </summary>
    /// <param name="tests">The test instance.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="tests"/> is <see langword="null"/>.</exception>
    public static void RunRegistrationTests(this ServiceCollectionExtensionsUnitTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.AddBlazorComponentLibrary_NullServices_ThrowsArgumentNullException();
        tests.AddBlazorComponentLibrary_NullConfigure_DoesNotThrow();
        tests.AddBlazorComponentLibrary_Idempotent_DoesNotDuplicateRegistrations();
    }
}