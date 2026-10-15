using System.Globalization;
using Microsoft.Extensions.Options;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Validates <see cref="ThemeOptions"/> when the host starts. Every failure is collected so the
/// resulting <see cref="Microsoft.Extensions.Options.OptionsValidationException"/> lists all problems at once.
/// </summary>
public sealed class ThemeOptionsValidator : IValidateOptions<ThemeOptions>
{
    /// <summary>
    /// Checks that the default theme is registered, that every CSS custom property name starts with
    /// <c>--</c>, that every colour value is a valid CSS hex or <c>rgb()</c>/<c>rgba()</c> string,
    /// and that a storage key is present when persistence is enabled.
    /// </summary>
    /// <param name="name">The options instance name. Ignored; the rules apply to every instance.</param>
    /// <param name="options">The options to validate.</param>
    /// <returns><see cref="ValidateOptionsResult.Success"/> when valid; otherwise a failure listing each problem.</returns>
    public ValidateOptionsResult Validate(string? name, ThemeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        ValidateDefaultTheme(options, failures);
        ValidateThemes(options, failures);

        if (options.EnablePersistence && string.IsNullOrWhiteSpace(options.StorageKey))
        {
            failures.Add("StorageKey must be non-empty when EnablePersistence is true.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }

    private static void ValidateDefaultTheme(ThemeOptions options, List<string> failures)
    {
        if (string.IsNullOrWhiteSpace(options.DefaultTheme))
        {
            failures.Add("DefaultTheme must be a non-empty theme name.");
            return;
        }

        var isRegistered = options.Themes is not null &&
            options.Themes.Keys.Any(key => string.Equals(key, options.DefaultTheme, StringComparison.OrdinalIgnoreCase));

        if (!isRegistered)
        {
            failures.Add($"DefaultTheme '{options.DefaultTheme}' is not one of the registered themes.");
        }
    }

    private static void ValidateThemes(ThemeOptions options, List<string> failures)
    {
        if (options.Themes is null)
        {
            failures.Add("Themes must not be null.");
            return;
        }

        foreach (var (themeName, theme) in options.Themes)
        {
            if (string.IsNullOrWhiteSpace(themeName))
            {
                failures.Add("Theme names must be non-empty.");
                continue;
            }

            if (theme?.CssVariables is null)
            {
                failures.Add($"Theme '{themeName}' must define CssVariables.");
                continue;
            }

            foreach (var (variableName, colour) in theme.CssVariables)
            {
                if (!variableName.StartsWith("--", StringComparison.Ordinal) || variableName.Length <= 2)
                {
                    failures.Add($"Theme '{themeName}': CSS variable '{variableName}' must start with '--' followed by a name.");
                }

                if (!IsValidCssColour(colour))
                {
                    failures.Add($"Theme '{themeName}': value '{colour}' for CSS variable '{variableName}' is not a valid CSS hex or rgb/rgba colour.");
                }
            }
        }
    }

    /// <summary>
    /// Returns <c>true</c> for <c>#rgb</c>, <c>#rgba</c>, <c>#rrggbb</c> and <c>#rrggbbaa</c> hex colours,
    /// and for <c>rgb(r, g, b[, a])</c> / <c>rgba(r, g, b[, a])</c> functions where the channels are all
    /// numbers in 0–255 or all percentages in 0–100%, and the optional alpha is 0–1 or 0–100%.
    /// </summary>
    private static bool IsValidCssColour(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var trimmed = value.Trim();

        if (trimmed[0] == '#')
        {
            var digits = trimmed.AsSpan(1);
            return digits.Length is 3 or 4 or 6 or 8 && IsAllHexDigits(digits);
        }

        var open = trimmed.IndexOf('(');
        if (open < 0 || trimmed[^1] != ')')
        {
            return false;
        }

        var function = trimmed[..open];
        if (!function.Equals("rgb", StringComparison.OrdinalIgnoreCase) &&
            !function.Equals("rgba", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var arguments = trimmed[(open + 1)..^1].Split(',');
        if (arguments.Length is not (3 or 4))
        {
            return false;
        }

        bool? channelsArePercent = null;
        for (var i = 0; i < 3; i++)
        {
            var channel = arguments[i].Trim();
            var isPercent = channel.EndsWith('%');

            // CSS does not allow mixing numeric and percentage channels in one colour.
            if (channelsArePercent is { } expected && expected != isPercent)
            {
                return false;
            }

            channelsArePercent = isPercent;

            if (!IsInRange(channel, isPercent ? 100 : 255))
            {
                return false;
            }
        }

        if (arguments.Length == 4)
        {
            var alpha = arguments[3].Trim();
            var isPercent = alpha.EndsWith('%');
            return IsInRange(alpha, isPercent ? 100 : 1);
        }

        return true;
    }

    private static bool IsInRange(string component, double max)
    {
        var isPercent = component.EndsWith('%');
        var numeric = isPercent ? component[..^1].TrimEnd() : component;

        return double.TryParse(numeric, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number)
            && number >= 0
            && number <= max;
    }

    private static bool IsAllHexDigits(ReadOnlySpan<char> digits)
    {
        foreach (var c in digits)
        {
            if (!char.IsAsciiHexDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}
