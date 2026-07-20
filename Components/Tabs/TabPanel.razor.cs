namespace BlazorComponentLibrary.Components.Tabs;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A single tab panel that displays content when active.
/// </summary>
public sealed partial class TabPanel : ComponentBase, IDisposable
{
    public void Dispose()
    {
        Parent?.UnregisterTabPanel(this);
    }
}
