using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using BlazorComponentLibrary.Services;
using BlazorComponentLibrary.Exceptions;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Additional tests for <see cref="ToastService"/> that focus on queue‑related behavior:
/// • No artificial limit on simultaneous toasts (the service currently allows any number).
/// • FIFO ordering of the <see cref="ToastService.ActiveToasts"/> collection.
/// • Dismissing a specific toast removes only that toast and preserves the order of the remaining items.
/// </summary>
public sealed class ToastServiceQueueTests : IDisposable
{
    private readonly ToastService _service;

    public ToastServiceQueueTests()
    {
        // Use the default constructor (no logger) – the service creates a no‑op logger internally.
        _service = new ToastService();
    }

    public void Dispose()
    {
        // Ensure timers are cleaned up after each test.
        _service.Dispose();
    }

    [Fact]
    public void MaxSimultaneousToasts_NoLimit_AllAdded()
    {
        // Arrange
        const int count = 20; // Arbitrary number well beyond any typical UI limit.

        // Act
        for (int i = 0; i < count; i++)
        {
            _service.Show($"Toast {i}");
        }

        // Assert
        Assert.Equal(count, _service.ActiveToasts.Count);
        // Verify that each expected message is present.
        var messages = _service.ActiveToasts.Select(t => t.Message).ToList();
        for (int i = 0; i < count; i++)
        {
            Assert.Contains($"Toast {i}", messages);
        }
    }

    [Fact]
    public void FIFO_Order_Maintained()
    {
        // Arrange
        var messages = new[] { "First", "Second", "Third", "Fourth" };
        foreach (var msg in messages)
        {
            _service.Show(msg);
        }

        // Act
        var active = _service.ActiveToasts;

        // Assert
        Assert.Equal(messages.Length, active.Count);
        for (int i = 0; i < messages.Length; i++)
        {
            Assert.Equal(messages[i], active[i].Message);
        }
    }

    [Fact]
    public void Dismiss_RemovesOnlyTargetToast_PreservesOrder()
    {
        // Arrange
        _service.Show("Alpha");
        _service.Show("Beta");
        _service.Show("Gamma");

        var toasts = _service.ActiveToasts.ToList();
        var betaId = toasts.First(t => t.Message == "Beta").Id;

        // Act
        _service.Dismiss(betaId);

        // Assert
        var remaining = _service.ActiveToasts;
        Assert.Equal(2, remaining.Count);
        // Ensure the remaining messages are Alpha and Gamma in the original order.
        Assert.Equal("Alpha", remaining[0].Message);
        Assert.Equal("Gamma", remaining[1].Message);
        // Verify that the dismissed toast is truly gone.
        Assert.DoesNotContain(remaining, t => t.Id == betaId);
    }

    [Fact]
    public void DismissAll_ClearsAll_ToastsAndTimers()
    {
        // Arrange
        _service.Show("One", durationMs: 1000);
        _service.Show("Two", durationMs: 1000);
        _service.Show("Three", durationMs: 1000);

        // Act
        _service.DismissAll();

        // Assert
        Assert.Empty(_service.ActiveToasts);
        // No exception should be thrown when calling Dismiss after the list is cleared.
        var ex = Record.Exception(() => _service.Dismiss(Guid.NewGuid()));
        Assert.Null(ex);
    }
}
