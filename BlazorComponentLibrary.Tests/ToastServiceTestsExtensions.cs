namespace BlazorComponentLibrary.Tests;

public static class ToastServiceTestsExtensions
{
    /// <summary>
    /// Executes all <c>Show</c>-related test scenarios in sequence.
    /// </summary>
    /// <param name="tests">The test class instance to execute scenarios on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void RunShowScenarios(this ToastServiceTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.Show_AddsToastToActiveList();
        tests.Show_SetsMessageAndType();
        tests.Show_EmptyMessage_ThrowsToastServiceException();
        tests.Show_RaisesToastsChangedEvent();
    }

    /// <summary>
    /// Executes all <c>Dismiss</c>-related test scenarios in sequence.
    /// </summary>
    /// <param name="tests">The test class instance to execute scenarios on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void RunDismissScenarios(this ToastServiceTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.Dismiss_RemovesCorrectToastById();
        tests.Dismiss_UnknownId_DoesNotThrow();
        tests.DismissAll_ClearsActiveToasts();
        tests.MultipleToasts_AreStoredInOrder();
    }

    /// <summary>
    /// Executes all toast service test scenarios in sequence.
    /// </summary>
    /// <param name="tests">The test class instance to execute scenarios on.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void RunAllScenarios(this ToastServiceTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        tests.RunShowScenarios();
        tests.RunDismissScenarios();
    }
}
