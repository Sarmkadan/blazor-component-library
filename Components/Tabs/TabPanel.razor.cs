namespace BlazorComponentLibrary.Components.Tabs;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A single tab panel that displays content when active.
/// </summary>
public sealed partial class TabPanel : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool _disposed;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Parent?.UnregisterTabPanel(this);
        _disposed = true;
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }
}
