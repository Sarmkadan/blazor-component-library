using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorComponentLibrary.Components.Toast;
using BlazorComponentLibrary.Services;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests for the <see cref="ToastContainer"/> component's public API.
/// </summary>
public sealed class ToastContainerTests : TestContext
{
    #region Helper types

    /// <summary>
    /// Minimal fake implementation of <see cref="IToastService"/> used for testing.
    /// </summary>
    private sealed class FakeToastService : IToastService
    {
        private readonly List<ToastMessage> _active = new();
        private Action? _toastsChanged;
        private int _subscriberCount;

        public IEnumerable<ToastMessage> ActiveToasts => _active;

        public event Action? ToastsChanged
        {
            add
            {
                _toastsChanged += value;
                _subscriberCount++;
            }
            remove
            {
                _toastsChanged -= value;
                _subscriberCount--;
            }
        }

        public int SubscriberCount => _subscriberCount;

        public void AddToast(ToastMessage toast)
        {
            _active.Add(toast);
            _toastsChanged?.Invoke();
        }
    }

    #endregion

    [Fact]
    public void IconFor_ReturnsCorrectIcon_ForEachToastType()
    {
        Assert.Equal("✓", ToastContainer.IconFor(ToastType.Success));
        Assert.Equal("⚠", ToastContainer.IconFor(ToastType.Warning));
        Assert.Equal("✕", ToastContainer.IconFor(ToastType.Error));
        // Any other value falls back to the info icon
        Assert.Equal("ℹ", ToastContainer.IconFor((ToastType)999));
    }

    [Fact]
    public void GetToastIcon_ReturnsCustomIcon_WhenProvided()
    {
        var toast = new ToastMessage { Type = ToastType.Success, Icon = "custom-icon" };
        var result = ToastContainer.GetToastIcon(toast);
        Assert.Equal("custom-icon", result);
    }

    [Fact]
    public void GetToastIcon_ReturnsDefaultIcon_WhenCustomIconIsNull()
    {
        var toast = new ToastMessage { Type = ToastType.Warning, Icon = null };
        var result = ToastContainer.GetToastIcon(toast);
        Assert.Equal("⚠", result);
    }

    [Fact]
    public void PositionParameter_ControlsRootCssClass()
    {
        // TopLeft
        var cut = RenderComponent<ToastContainer>(parameters => parameters
            .Add(p => p.Position, ToastPosition.TopLeft));

        var rootDiv = cut.Find("div");
        Assert.Contains("bcl-toast-container--top-left", rootDiv.ClassList);

        // BottomRight (default)
        cut = RenderComponent<ToastContainer>();
        rootDiv = cut.Find("div");
        Assert.Contains("bcl-toast-container--bottom-right", rootDiv.ClassList);
    }

    [Fact]
    public void MaxVisible_DefaultValue_IsFive()
    {
        var cut = RenderComponent<ToastContainer>();
        Assert.Equal(5, cut.Instance.MaxVisible);
    }

    [Fact]
    public void Position_DefaultValue_IsBottomRight()
    {
        var cut = RenderComponent<ToastContainer>();
        Assert.Equal(ToastPosition.BottomRight, cut.Instance.Position);
    }

    [Fact]
    public void VisibleToasts_ReturnsLastNToasts_BasedOnMaxVisible()
    {
        var fakeService = new FakeToastService();

        // Add 7 toasts
        for (int i = 1; i <= 7; i++)
        {
            fakeService.AddToast(new ToastMessage { Type = ToastType.Info, Icon = $"icon{i}" });
        }

        // Register the fake service in the test DI container
        Services.AddSingleton<IToastService>(fakeService);

        var cut = RenderComponent<ToastContainer>(parameters => parameters
            .Add(p => p.MaxVisible, 5));

        // Use reflection to get the internal VisibleToasts property
        var prop = typeof(ToastContainer).GetProperty(
            "VisibleToasts",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        Assert.NotNull(prop);

        var visible = (IEnumerable<ToastMessage>)prop!.GetValue(cut.Instance)!;
        var visibleArray = visible.ToArray();

        Assert.Equal(5, visibleArray.Length);
        // Should be the last 5 added (i.e., 3..7)
        var expectedIcons = new[] { "icon3", "icon4", "icon5", "icon6", "icon7" };
        Assert.Equal(expectedIcons, visibleArray.Select(t => t.Icon));
    }

    [Fact]
    public void Dispose_UnsubscribesFromToastService()
    {
        var fakeService = new FakeToastService();
        Services.AddSingleton<IToastService>(fakeService);

        var cut = RenderComponent<ToastContainer>();

        // After initialization the component should have subscribed once
        Assert.Equal(1, fakeService.SubscriberCount);

        // Dispose the component
        cut.Instance.Dispose();

        // Subscription should be removed
        Assert.Equal(0, fakeService.SubscriberCount);
    }
}
