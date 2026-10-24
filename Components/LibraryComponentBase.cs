namespace BlazorComponentLibrary.Components;

using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Base class for every public component in the library. It defines the shared parameter contract:
/// <see cref="Class"/> for consumer-supplied CSS classes and <see cref="AdditionalAttributes"/> for
/// attributes that are splatted onto the component's root element. Derived components build their
/// class attribute with <see cref="BuildCssClass"/> so the library's <c>bcl-*</c> tokens and the
/// consumer's classes are combined the same way everywhere.
/// </summary>
public abstract class LibraryComponentBase : ComponentBase
{
    /// <summary>
    /// Gets or sets extra CSS class names appended to the component's root element after the
    /// library's own classes. Null, empty, or whitespace-only values add nothing.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Gets or sets attributes supplied to the component that do not match a declared parameter,
    /// such as <c>aria-label</c> or <c>data-testid</c>. They are rendered onto the root element.
    /// A <c>class</c> entry is merged into the root element's class list rather than splatted, so it
    /// adds to the library's classes instead of replacing them. Attributes the component renders itself
    /// (for example <c>role</c> or <c>aria-label</c>) take precedence over a splatted value of the same name.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>
    /// Gets the <see cref="AdditionalAttributes"/> entries that should be splatted onto the root element.
    /// The <c>class</c> entry is excluded because <see cref="BuildCssClass"/> already merges it.
    /// </summary>
    protected IEnumerable<KeyValuePair<string, object>>? RootAttributes =>
        AdditionalAttributes?.Where(static attribute => !IsClassAttribute(attribute.Key));

    /// <summary>
    /// Builds the value of a root element's <c>class</c> attribute from the library's own tokens,
    /// followed by <see cref="Class"/>, then any <c>class</c> entry supplied through <see cref="AdditionalAttributes"/>.
    /// Tokens are trimmed, blank tokens are skipped, and the result is separated by single spaces so that
    /// theme and modifier classes stay predictable.
    /// </summary>
    /// <param name="tokens">Library CSS class tokens in the order they should be emitted. Null or blank entries are ignored.</param>
    /// <returns>
    /// The space-separated class list, or <see langword="null"/> when there are no classes so that the
    /// attribute is omitted from the rendered markup.
    /// </returns>
    protected string? BuildCssClass(params ReadOnlySpan<string?> tokens)
    {
        var builder = new StringBuilder();
        foreach (var token in tokens)
        {
            AppendToken(builder, token);
        }

        AppendToken(builder, Class);
        if (AdditionalAttributes is not null)
        {
            foreach (var (key, value) in AdditionalAttributes)
            {
                if (IsClassAttribute(key))
                {
                    AppendToken(builder, value?.ToString());
                }
            }
        }

        return builder.Length == 0 ? null : builder.ToString();
    }

    private static bool IsClassAttribute(string key) =>
        string.Equals(key, "class", StringComparison.OrdinalIgnoreCase);

    private static void AppendToken(StringBuilder builder, string? token)
    {
        var trimmed = token?.Trim();
        if (string.IsNullOrEmpty(trimmed))
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append(' ');
        }

        builder.Append(trimmed);
    }
}
