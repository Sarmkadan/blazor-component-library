using System;
using System.Collections.Generic;
using BlazorComponentLibrary;

namespace BlazorComponentLibrary.Examples;

/// <summary>
/// Demonstrates advanced usage including theme management and list interaction.
/// </summary>
public class AdvancedUsage
{
    private readonly IThemeService _themeService;

    public AdvancedUsage(IThemeService themeService)
    {
        _themeService = themeService;
    }

    public void ToggleToDarkTheme()
    {
        // Programmatically set theme
        _themeService.SetTheme(ThemeMode.Dark);
    }

    public void SubscribeToThemeChanges()
    {
        // Subscribe to changes
        _themeService.ThemeChanged += (mode) =>
        {
            Console.WriteLine($"Theme changed to: {mode}");
        };
    }

    public void HandleOrderChanged(IList<string> updatedItems)
    {
        // Process reordered list from DragDropList
        foreach (var item in updatedItems)
        {
            Console.WriteLine($"Item: {item}");
        }
    }
}
