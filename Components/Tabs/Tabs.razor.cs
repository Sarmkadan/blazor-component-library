namespace BlazorComponentLibrary.Components.Tabs;

using Microsoft.AspNetCore.Components;

/// <summary>
/// A tabbed interface component that allows switching between different panels.
/// </summary>
public sealed partial class Tabs : ComponentBase, IDisposable
{
    public void Dispose()
    {
        _tabPanels.Clear();
    }
}
