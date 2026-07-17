namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Services;

/// <summary>
/// Tests for the ToastService class.
/// </summary>
public sealed class ToastServiceTests
{
    /// <summary>
    /// Verifies that the Show method adds a toast to the active list.
    /// </summary>
    [Fact]
    public void Show_AddsToastToActiveList()
    {
        var service = new ToastService();
        service.Show("Hello world");
        Assert.Single(service.ActiveToasts);
    }

    /// <summary>
    /// Verifies that the Show method sets the message and type of the toast.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void Show_SetsMessageAndType()
    {
        var service = new ToastService();
        service.Show("Saved!", ToastType.Success);

        var toast = service.ActiveToasts[0];
        Assert.Equal("Saved!", toast.Message);
        Assert.Equal(ToastType.Success, toast.Type);
    }

    /// <summary>
    /// Verifies that the Show method throws a ToastServiceException when an empty message is provided.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void Show_EmptyMessage_ThrowsToastServiceException()
    {
        var service = new ToastService();
        Assert.Throws<BlazorComponentLibrary.Exceptions.ToastServiceException>(() => service.Show("   "));
    }

    /// <summary>
    /// Verifies that the Show method raises the ToastsChanged event.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void Show_RaisesToastsChangedEvent()
    {
        var service = new ToastService();
        var raised = false;
        service.ToastsChanged += () => raised = true;

        service.Show("Test notification");

        Assert.True(raised);
    }

    /// <summary>
    /// Verifies that the Dismiss method removes the correct toast by its ID.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void Dismiss_RemovesCorrectToastById()
    {
        var service = new ToastService();
        service.Show("First");
        service.Show("Second");
        var idToRemove = service.ActiveToasts[0].Id;

        service.Dismiss(idToRemove);

        Assert.Single(service.ActiveToasts);
        Assert.DoesNotContain(service.ActiveToasts, t => t.Id == idToRemove);
    }

    /// <summary>
    /// Verifies that the Dismiss method does not throw an exception when an unknown ID is provided.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void Dismiss_UnknownId_DoesNotThrow()
    {
        var service = new ToastService();
        service.Show("One");
        var exception = Record.Exception(() => service.Dismiss(Guid.NewGuid()));
        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that the DismissAll method clears the active toasts list.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void DismissAll_ClearsActiveToasts()
    {
        var service = new ToastService();
        service.Show("A");
        service.Show("B");
        service.Show("C");

        service.DismissAll();

        Assert.Empty(service.ActiveToasts);
    }

    /// <summary>
    /// Verifies that multiple toasts are stored in the correct order.
    /// </summary>
    /// <param name="service">The ToastService instance to test.</param>
    [Fact]
    public void MultipleToasts_AreStoredInOrder()
    {
        var service = new ToastService();
        service.Show("First",  ToastType.Info,    0);
        service.Show("Second", ToastType.Warning, 0);
        service.Show("Third",  ToastType.Error,   0);

        Assert.Equal(3, service.ActiveToasts.Count);
        Assert.Equal("First",  service.ActiveToasts[0].Message);
        Assert.Equal("Second", service.ActiveToasts[1].Message);
        Assert.Equal("Third",  service.ActiveToasts[2].Message);
    }
}
