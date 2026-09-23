namespace BlazorComponentLibrary.Tests;

using System;
using global::BlazorComponentLibrary.Components.Modal;
using Bunit;

/// <summary>
/// Extension methods to simplify rendering and asserting the <see cref="Modal"/> component in unit tests.
/// </summary>
public static class ModalRazorUnitTestsExtensions
{
    /// <summary>
    /// Renders the <see cref="Modal"/> component with the specified parameters for testing.
    /// </summary>
    /// <param name="testContext">The test context instance.</param>
    /// <param name="title">The title to set on the modal.</param>
    /// <param name="isVisible">Whether the modal is initially visible.</param>
    /// <param name="size">The size variant of the modal.</param>
    /// <param name="closeOnOverlayClick">Whether clicking the overlay closes the modal.</param>
    /// <returns>The rendered component instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testContext"/> is null.</exception>
    public static IRenderedComponent<Modal> RenderModalWithParameters(
        this ModalRazorUnitTests testContext,
        string? title = null,
        bool isVisible = false,
        ModalSize size = ModalSize.Medium,
        bool closeOnOverlayClick = true)
    {
        ArgumentNullException.ThrowIfNull(testContext);

        return testContext.RenderComponent<Modal>(parameters => parameters
            .Add(p => p.Title, title)
            .Add(p => p.IsVisible, isVisible)
            .Add(p => p.Size, size)
            .Add(p => p.CloseOnOverlayClick, closeOnOverlayClick));
    }

    /// <summary>
    /// Asserts that the rendered modal markup contains the expected title text.
    /// </summary>
    /// <param name="testContext">The test context instance.</param>
    /// <param name="renderedComponent">The rendered modal component.</param>
    /// <param name="expectedTitle">The expected title string.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testContext"/> or <paramref name="renderedComponent"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="expectedTitle"/> is null or whitespace.</exception>
    public static void AssertModalContainsTitle(
        this ModalRazorUnitTests testContext,
        IRenderedComponent<Modal> renderedComponent,
        string expectedTitle)
    {
        ArgumentNullException.ThrowIfNull(testContext);
        ArgumentNullException.ThrowIfNull(renderedComponent);
        ArgumentException.ThrowIfNullOrEmpty(expectedTitle);

        var markup = renderedComponent.Markup;
        if (!markup.Contains(expectedTitle, StringComparison.Ordinal))
        {
            throw new ArgumentException(
                $"Expected markup to contain '{expectedTitle}', but it did not.\nActual markup:\n{markup}");
        }
    }

    /// <summary>
    /// Asserts that the modal's visibility state matches the expected value.
    /// </summary>
    /// <param name="testContext">The test context instance.</param>
    /// <param name="renderedComponent">The rendered modal component.</param>
    /// <param name="expectedVisibility">The expected visibility state.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testContext"/> or <paramref name="renderedComponent"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the actual visibility does not match the expected value.</exception>
    public static void AssertModalVisibilityState(
        this ModalRazorUnitTests testContext,
        IRenderedComponent<Modal> renderedComponent,
        bool expectedVisibility)
    {
        ArgumentNullException.ThrowIfNull(testContext);
        ArgumentNullException.ThrowIfNull(renderedComponent);

        var actualVisibility = renderedComponent.Instance.IsVisible;
        if (actualVisibility != expectedVisibility)
        {
            throw new InvalidOperationException(
                $"Expected modal visibility to be {expectedVisibility}, but was {actualVisibility}.");
        }
    }
}
