namespace BlazorComponentLibrary.Tests;

using Xunit;
using BlazorComponentLibrary.Services;
using BlazorComponentLibrary.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using FluentAssertions;

/// <summary>
/// Tests for the ServiceCollectionExtensions class.
/// </summary>
public sealed class ServiceCollectionExtensionsUnitTests
{
    /// <summary>
    /// Verifies that AddBlazorComponentLibrary returns the same service collection instance for chaining.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddBlazorComponentLibrary();

        Assert.Same(services, result);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary registers IThemeService with ThemeService implementation.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_RegistersIThemeService_WithThemeServiceImplementation()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        // Mock IJSRuntime for ThemeService
        var jsMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsMock.Object);

        using var serviceProvider = services.BuildServiceProvider();

        var themeService = serviceProvider.GetService<IThemeService>();
        Assert.NotNull(themeService);
        Assert.IsType<ThemeService>(themeService);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary registers IToastService with ToastService implementation.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_RegistersIToastService_WithToastServiceImplementation()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        using var serviceProvider = services.BuildServiceProvider();

        var toastService = serviceProvider.GetService<IToastService>();
        Assert.NotNull(toastService);
        Assert.IsType<ToastService>(toastService);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary registers services as scoped.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_RegistersServicesAsScoped()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        var serviceDescriptors = services.Where(s => s.ServiceType == typeof(IThemeService) || s.ServiceType == typeof(IToastService));

        foreach (var descriptor in serviceDescriptors)
        {
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary registers services only once (TryAdd behavior).
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_RegistersServicesOnlyOnce()
    {
        var services = new ServiceCollection();

        // Call twice
        services.AddBlazorComponentLibrary();
        services.AddBlazorComponentLibrary();

        // Mock IJSRuntime for ThemeService
        var jsMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsMock.Object);

        using var serviceProvider = services.BuildServiceProvider();

        var themeService1 = serviceProvider.GetService<IThemeService>();
        var themeService2 = serviceProvider.GetService<IThemeService>();
        var toastService1 = serviceProvider.GetService<IToastService>();
        var toastService2 = serviceProvider.GetService<IToastService>();

        Assert.Same(themeService1, themeService2);
        Assert.Same(toastService1, toastService2);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary registers logging services.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_RegistersLoggingServices()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        var loggers = services.Where(s => s.ServiceType == typeof(ILogger<>));
        Assert.NotEmpty(loggers);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary throws ArgumentNullException when services is null.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection? services = null;

        Assert.Throws<ArgumentNullException>(() => services!.AddBlazorComponentLibrary());
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary allows custom IThemeService registration before calling it.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_AllowsCustomThemeServiceRegistration()
    {
        var services = new ServiceCollection();

        // Register custom implementation first
        services.AddScoped<IThemeService, CustomThemeService>();

        // Then call AddBlazorComponentLibrary
        services.AddBlazorComponentLibrary();

        using var serviceProvider = services.BuildServiceProvider();

        var themeService = serviceProvider.GetService<IThemeService>();
        Assert.NotNull(themeService);
        Assert.IsType<CustomThemeService>(themeService);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary allows custom IToastService registration before calling it.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_AllowsCustomToastServiceRegistration()
    {
        var services = new ServiceCollection();

        // Register custom implementation first
        services.AddScoped<IToastService, CustomToastService>();

        // Then call AddBlazorComponentLibrary
        services.AddBlazorComponentLibrary();

        using var serviceProvider = services.BuildServiceProvider();

        var toastService = serviceProvider.GetService<IToastService>();
        Assert.NotNull(toastService);
        Assert.IsType<CustomToastService>(toastService);
    }

    /// <summary>
    /// Verifies that services can be resolved from the service provider after registration.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_ServicesResolvableFromProvider()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        // Mock IJSRuntime for ThemeService
        var jsMock = new Mock<IJSRuntime>();
        services.AddSingleton(jsMock.Object);

        using var serviceProvider = services.BuildServiceProvider();

        var themeService = serviceProvider.GetRequiredService<IThemeService>();
        var toastService = serviceProvider.GetRequiredService<IToastService>();

        Assert.NotNull(themeService);
        Assert.NotNull(toastService);
    }

    /// <summary>
    /// Verifies that AddBlazorComponentLibrary adds services to the service collection.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_AddsServicesToCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddBlazorComponentLibrary();

        Assert.NotNull(result);
        Assert.NotEmpty(services);
    }

    /// <summary>
    /// Custom IThemeService implementation for testing TryAdd behavior.
    /// </summary>
    private sealed class CustomThemeService : IThemeService
    {
        public ThemeMode CurrentTheme { get; private set; } = ThemeMode.System;

        public event Action<ThemeMode>? ThemeChanged;
        public event Action<string>? OnThemeChanged;

        public void SetTheme(ThemeMode mode) => CurrentTheme = mode;
        public Task InitializeAsync() => Task.CompletedTask;
    }

    /// <summary>
    /// Custom IToastService implementation for testing TryAdd behavior.
    /// </summary>
    private sealed class CustomToastService : IToastService
    {
        public IReadOnlyList<ToastMessage> ActiveToasts => _activeToasts.AsReadOnly();
        private readonly List<ToastMessage> _activeToasts = [];
        public event Action? ToastsChanged;

        public void Dismiss(Guid id) => _activeToasts.RemoveAll(t => t.Id == id);
        public void DismissAll() => _activeToasts.Clear();
        public void Show(string message, ToastType type = ToastType.Info, int durationMs = 5000, string? icon = null)
        {
            var toast = new ToastMessage(Guid.NewGuid(), message, type, durationMs, icon);
            _activeToasts.Add(toast);
            ToastsChanged?.Invoke();
        }
        public void PauseTimer(Guid id) { }
        public void ResumeTimer(Guid id, double remainingMs) { }
    }
}
