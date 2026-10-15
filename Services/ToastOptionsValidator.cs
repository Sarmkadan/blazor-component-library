using BlazorComponentLibrary.Components.Toast;
using Microsoft.Extensions.Options;

namespace BlazorComponentLibrary.Services;

/// <summary>
/// Validates <see cref="ToastOptions"/> when the host starts. Every failure is collected so the
/// resulting <see cref="Microsoft.Extensions.Options.OptionsValidationException"/> lists all problems at once.
/// </summary>
public sealed class ToastOptionsValidator : IValidateOptions<ToastOptions>
{
    /// <summary>Smallest accepted <see cref="ToastOptions.DefaultDuration"/>.</summary>
    public static readonly TimeSpan MinDefaultDuration = TimeSpan.FromMilliseconds(500);

    /// <summary>Largest accepted <see cref="ToastOptions.DefaultDuration"/>.</summary>
    public static readonly TimeSpan MaxDefaultDuration = TimeSpan.FromSeconds(60);

    /// <summary>Smallest accepted <see cref="ToastOptions.MaxVisibleToasts"/>.</summary>
    public const int MinMaxVisibleToasts = 1;

    /// <summary>Largest accepted <see cref="ToastOptions.MaxVisibleToasts"/>.</summary>
    public const int MaxMaxVisibleToasts = 20;

    /// <summary>
    /// Checks that <see cref="ToastOptions.DefaultDuration"/> is between 500 ms and 60 s,
    /// <see cref="ToastOptions.MaxVisibleToasts"/> is between 1 and 20, and
    /// <see cref="ToastOptions.Position"/> is a defined <see cref="ToastPosition"/> value.
    /// </summary>
    /// <param name="name">The options instance name. Ignored; the rules apply to every instance.</param>
    /// <param name="options">The options to validate.</param>
    /// <returns><see cref="ValidateOptionsResult.Success"/> when valid; otherwise a failure listing each problem.</returns>
    public ValidateOptionsResult Validate(string? name, ToastOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var failures = new List<string>();

        if (options.DefaultDuration < MinDefaultDuration || options.DefaultDuration > MaxDefaultDuration)
        {
            failures.Add($"DefaultDuration must be between {MinDefaultDuration.TotalMilliseconds} ms and {MaxDefaultDuration.TotalSeconds} s, but was {options.DefaultDuration}.");
        }

        if (options.MaxVisibleToasts < MinMaxVisibleToasts || options.MaxVisibleToasts > MaxMaxVisibleToasts)
        {
            failures.Add($"MaxVisibleToasts must be between {MinMaxVisibleToasts} and {MaxMaxVisibleToasts}, but was {options.MaxVisibleToasts}.");
        }

        if (!Enum.IsDefined(options.Position))
        {
            failures.Add($"Position '{options.Position}' is not a defined ToastPosition value.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
