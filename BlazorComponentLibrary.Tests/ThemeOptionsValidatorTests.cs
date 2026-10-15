using BlazorComponentLibrary.Services;
using Xunit;

namespace BlazorComponentLibrary.Tests;

/// <summary>
/// Tests for <see cref="ThemeOptionsValidator"/>, covering one valid configuration and each invalid rule.
/// </summary>
public sealed class ThemeOptionsValidatorTests
{
    private static readonly ThemeOptionsValidator Validator = new();

    /// <summary>
    /// Verifies that the default options (with the <c>light</c> and <c>dark</c> themes registered)
    /// pass validation.
    /// </summary>
    [Fact]
    public void Validate_DefaultOptions_Succeeds()
    {
        // Arrange
        var options = new ThemeOptions();

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    /// <summary>
    /// Verifies that a default theme that is not among the registered themes fails validation.
    /// </summary>
    [Fact]
    public void Validate_DefaultThemeNotRegistered_Fails()
    {
        // Arrange
        var options = new ThemeOptions { DefaultTheme = "brand" };

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("DefaultTheme 'brand' is not one of the registered themes"));
    }

    /// <summary>
    /// Verifies that a CSS custom property name without the leading <c>--</c> fails validation.
    /// </summary>
    [Fact]
    public void Validate_CssVariableWithoutDoubleDash_Fails()
    {
        // Arrange
        var options = new ThemeOptions();
        options.Themes["light"].CssVariables["bcl-primary"] = "#1e90ff";

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("CSS variable 'bcl-primary' must start with '--'"));
    }

    /// <summary>
    /// Verifies that colour values which are not CSS hex or <c>rgb()</c>/<c>rgba()</c> strings fail validation.
    /// </summary>
    /// <param name="colour">An invalid colour value.</param>
    [Theory]
    [InlineData("red")]
    [InlineData("#12345")]
    [InlineData("#ggg")]
    [InlineData("rgb(300, 0, 0)")]
    [InlineData("rgb(10, 20)")]
    [InlineData("rgb(10%, 20, 30)")]
    [InlineData("hsl(120, 50%, 50%)")]
    [InlineData("")]
    public void Validate_InvalidColourValue_Fails(string colour)
    {
        // Arrange
        var options = new ThemeOptions();
        options.Themes["dark"].CssVariables["--bcl-surface"] = colour;

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("is not a valid CSS hex or rgb/rgba colour"));
    }

    /// <summary>
    /// Verifies that hex and <c>rgb()</c>/<c>rgba()</c> colour values are accepted.
    /// </summary>
    /// <param name="colour">A valid colour value.</param>
    [Theory]
    [InlineData("#fff")]
    [InlineData("#1e90ff80")]
    [InlineData("rgb(30, 144, 255)")]
    [InlineData("rgba(30, 144, 255, 0.5)")]
    [InlineData("rgb(10%, 20%, 30%)")]
    public void Validate_ValidColourValue_Succeeds(string colour)
    {
        // Arrange
        var options = new ThemeOptions();
        options.Themes["dark"].CssVariables["--bcl-surface"] = colour;

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Succeeded);
    }

    /// <summary>
    /// Verifies that an empty storage key fails validation when persistence is enabled.
    /// </summary>
    [Fact]
    public void Validate_EmptyStorageKeyWithPersistence_Fails()
    {
        // Arrange
        var options = new ThemeOptions { EnablePersistence = true, StorageKey = "  " };

        // Act
        var result = Validator.Validate(null, options);

        // Assert
        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("StorageKey must be non-empty when EnablePersistence is true"));
    }
}
