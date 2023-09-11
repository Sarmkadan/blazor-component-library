namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Services;

public sealed class ToastServiceTests
{
    [Fact]
    public void Show_AddsToastToActiveList()
    {
        var service = new ToastService();
        service.Show("Hello world");
        Assert.Single(service.ActiveToasts);
    }

    [Fact]
    public void Show_SetsMessageAndType()
    {
        var service = new ToastService();
        service.Show("Saved!", ToastType.Success);

        var toast = service.ActiveToasts[0];
        Assert.Equal("Saved!", toast.Message);
        Assert.Equal(ToastType.Success, toast.Type);
    }

    [Fact]
    public void Show_EmptyMessage_ThrowsArgumentException()
    {
        var service = new ToastService();
        Assert.Throws<ArgumentException>(() => service.Show("   "));
    }

    [Fact]
    public void Show_RaisesToastsChangedEvent()
    {
        var service = new ToastService();
        var raised = false;
        service.ToastsChanged += () => raised = true;

        service.Show("Test notification");

        Assert.True(raised);
    }

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

    [Fact]
    public void Dismiss_UnknownId_DoesNotThrow()
    {
        var service = new ToastService();
        service.Show("One");
        var exception = Record.Exception(() => service.Dismiss(Guid.NewGuid()));
        Assert.Null(exception);
    }

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
