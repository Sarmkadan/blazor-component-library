namespace BlazorComponentLibrary.Components.Tabs;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A tabbed interface component that allows switching between different panels.
/// </summary>
public sealed partial class Tabs : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool _disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _tabPanels.Clear();
        _disposed = true;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
