// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using BlazorComponentLibrary.Models;
using BlazorComponentLibrary.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorComponentLibrary.Infrastructure;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods for registering the complete
/// virtualized data grid subsystem into the application's dependency injection container.
/// </summary>
public static class VirtualizedGridServiceExtensions
{
    /// <summary>
    /// Registers all services required by the virtualized data grid engine and optionally
    /// applies the <paramref name="configure"/> delegate to tune the
    /// <see cref="VirtualizedGridOptions"/> singleton before it is sealed.
    /// <para>
    /// The following registrations are added when this method is called:
    /// <list type="table">
    ///   <listheader>
    ///     <term>Service</term>
    ///     <description>Implementation / Lifetime</description>
    ///   </listheader>
    ///   <item>
    ///     <term><see cref="VirtualizedGridOptions"/></term>
    ///     <description><see cref="VirtualizedGridOptions"/> — Singleton</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="IGridEditHandler"/></term>
    ///     <description><see cref="InMemoryGridEditHandler"/> — Scoped</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="IVirtualizedGridService"/></term>
    ///     <description><see cref="VirtualizedGridService"/> — Scoped</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="IGridAggregationService"/></term>
    ///     <description><see cref="GridAggregationService"/> — Scoped</description>
    ///   </item>
    ///   <item>
    ///     <term><see cref="IGridExportService"/></term>
    ///     <description><see cref="GridExportService"/> — Scoped</description>
    ///   </item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Prerequisites:</b> <c>ICacheService</c> and <c>IDataRepository</c> must already be
    /// registered in <paramref name="services"/> before this method is called.  Both are
    /// included automatically when <see cref="ServiceRegistry.AddBlazorComponentLibrary"/> is
    /// used.  To register the grid subsystem independently, call
    /// <c>services.AddCaching()</c> and register a data repository first.
    /// </para>
    /// </summary>
    /// <param name="services">The application service collection to configure.</param>
    /// <param name="configure">
    ///   Optional delegate that receives a freshly-constructed <see cref="VirtualizedGridOptions"/>
    ///   and may mutate any of its properties before the instance is registered as a singleton.
    ///   When <c>null</c>, default option values are used.
    /// </param>
    /// <returns>
    ///   The same <see cref="IServiceCollection"/> for fluent method chaining.
    /// </returns>
    /// <example>
    /// <code>
    /// builder.Services.AddVirtualizedGrid(options =>
    /// {
    ///     options.EditMode              = GridEditMode.SingleCell;
    ///     options.SelectionMode         = GridSelectionMode.Multiple;
    ///     options.EnableColumnResizing  = true;
    ///     options.EnableGrouping        = true;
    ///     options.MaxInlineEditHistory  = 100;
    ///     options.CacheExpiration       = TimeSpan.FromMinutes(10);
    ///     options.VirtualScroll         = new VirtualScrollConfig(RowHeight: 48, PageSize: 100);
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddVirtualizedGrid(
        this IServiceCollection services,
        Action<VirtualizedGridOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new VirtualizedGridOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<IGridEditHandler, InMemoryGridEditHandler>();
        services.AddScoped<IVirtualizedGridService, VirtualizedGridService>();
        services.AddScoped<IGridAggregationService, GridAggregationService>();
        services.AddScoped<IGridExportService, GridExportService>();

        return services;
    }
}
