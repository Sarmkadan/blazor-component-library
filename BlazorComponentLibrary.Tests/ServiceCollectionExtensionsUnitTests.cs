using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using BlazorComponentLibrary.Extensions;
using BlazorComponentLibrary.Services;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests for <see cref="ServiceCollectionExtensions"/> ensuring registration behaves correctly
/// under various scenarios such as null arguments, idempotent calls, and service resolvability.
/// </summary>
public sealed class ServiceCollectionExtensionsUnitTests
{
    /// <summary>
    /// Verifies that passing a <c>null</c> <see cref="IServiceCollection"/> throws an
    /// <see cref="ArgumentNullException"/> with the correct parameter name.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_NullServices_ThrowsArgumentNullException()
    {
        IServiceCollection services = null!;

        var exception = Assert.Throws<ArgumentNullException>(
            () => ServiceCollectionExtensions.AddBlazorComponentLibrary(services));

        Assert.Equal("services", exception.ParamName);
    }

    /// <summary>
    /// Ensures that providing a <c>null</c> configuration delegate does not cause an exception
    /// and still returns the original service collection for chaining.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_NullConfigure_DoesNotThrow()
    {
        var services = new ServiceCollection();

        var result = services.AddBlazorComponentLibrary(null);

        Assert.Same(services, result);
    }

    /// <summary>
    /// Confirms that invoking the registration method multiple times on the same
    /// <see cref="IServiceCollection"/> does not create duplicate registrations for
    /// library services (i.e., the operation is idempotent).
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_Idempotent_DoesNotDuplicateRegistrations()
    {
        var services = new ServiceCollection();

        // First registration
        services.AddBlazorComponentLibrary();

        // Second registration – should not add new descriptors for the same service types
        services.AddBlazorComponentLibrary();

        var themeServiceCount = services.Count(d => d.ServiceType == typeof(IThemeService));
        var toastServiceCount = services.Count(d => d.ServiceType == typeof(IToastService));

        Assert.Equal(1, themeServiceCount);
        Assert.Equal(1, toastServiceCount);
    }

    /// <summary>
    /// Verifies that after registration the expected services can be resolved from a built
    /// <see cref="IServiceProvider"/>.
    /// </summary>
    [Fact]
    public void AddBlazorComponentLibrary_ResolvesServices()
    {
        var services = new ServiceCollection();

        services.AddBlazorComponentLibrary();

        using var provider = services.BuildServiceProvider();

        var themeService = provider.GetRequiredService<IThemeService>();
        var toastService = provider.GetRequiredService<IToastService>();

        Assert.NotNull(themeService);
        Assert.NotNull(toastService);
    }
}
