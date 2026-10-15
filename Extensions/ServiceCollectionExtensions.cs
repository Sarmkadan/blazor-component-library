using System;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BlazorComponentLibrary.Extensions;

/// <summary>
/// Extension methods for registering library services with the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all BlazorComponentLibrary services with the <see cref="IServiceCollection"/>.
    /// Calls the overload with no configuration delegate.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddBlazorComponentLibrary(this IServiceCollection services) =>
        services.AddBlazorComponentLibrary(null);

    /// <summary>
    /// Registers all BlazorComponentLibrary services with the <see cref="IServiceCollection"/>,
    /// allowing optional configuration of library‑specific options.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">
    /// An optional delegate to configure <see cref="BlazorComponentLibraryOptions"/>.
    /// If <c>null</c>, default options are used.
    /// </param>
    /// <param name="configureToast">
    /// An optional delegate to configure <see cref="ToastOptions"/>. The result is validated by
    /// <see cref="ToastOptionsValidator"/> when the host starts.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    /// <exception cref="Microsoft.Extensions.Options.OptionsValidationException">
    /// Thrown when the host starts with an invalid <see cref="ToastOptions"/> configuration.
    /// </exception>
    public static IServiceCollection AddBlazorComponentLibrary(
        this IServiceCollection services,
        Action<BlazorComponentLibraryOptions>? configure,
        Action<ToastOptions>? configureToast = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new BlazorComponentLibraryOptions();
        configure?.Invoke(options);

        var toastOptionsBuilder = services.AddOptions<ToastOptions>();

        if (configureToast is not null)
        {
            toastOptionsBuilder.Configure(configureToast);
        }

        toastOptionsBuilder.ValidateOnStart();

        // TryAddEnumerable keeps the validator to a single registration when this method is called twice.
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ToastOptions>, ToastOptionsValidator>());

        // TryAdd so an application can register its own IThemeService/IToastService
        // implementation before calling this and the library will pick it up.
        services.TryAddScoped<IThemeService, ThemeService>();
        services.TryAddScoped<IToastService, ToastService>();

        if (options.RegisterLogging)
        {
            services.AddLogging();
        }

        return services;
    }

    /// <summary>
    /// Registers <see cref="ThemeOptions"/> with the <see cref="IServiceCollection"/> using default values.
    /// The options are validated when the host starts; an invalid configuration throws an
    /// <see cref="Microsoft.Extensions.Options.OptionsValidationException"/>.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    public static IServiceCollection AddBlazorComponentLibraryTheme(this IServiceCollection services) =>
        services.AddBlazorComponentLibraryTheme(null);

    /// <summary>
    /// Registers <see cref="ThemeOptions"/> with the <see cref="IServiceCollection"/>, applies the optional
    /// <paramref name="configure"/> delegate, and validates the result with <see cref="ThemeOptionsValidator"/>
    /// when the host starts (<see cref="OptionsBuilderExtensions.ValidateOnStart{TOptions}"/>).
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="configure">
    /// An optional delegate to configure <see cref="ThemeOptions"/>.
    /// If <c>null</c>, default options are used.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is <c>null</c>.</exception>
    /// <exception cref="Microsoft.Extensions.Options.OptionsValidationException">
    /// Thrown when the host starts with an invalid <see cref="ThemeOptions"/> configuration.
    /// </exception>
    public static IServiceCollection AddBlazorComponentLibraryTheme(
        this IServiceCollection services,
        Action<ThemeOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(services);

        var optionsBuilder = services.AddOptions<ThemeOptions>();

        if (configure is not null)
        {
            optionsBuilder.Configure(configure);
        }

        optionsBuilder.ValidateOnStart();

        // TryAddEnumerable keeps the validator to a single registration when this method is called twice.
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<ThemeOptions>, ThemeOptionsValidator>());

        return services;
    }
}
