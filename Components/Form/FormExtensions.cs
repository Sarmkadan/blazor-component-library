namespace BlazorComponentLibrary.Components.Form;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Provides extension methods for working with <see cref="Form{TModel}"/> components.
/// </summary>
public static class FormExtensions
{
    /// <summary>
    /// Gets the first validation error message for the specified property, or null if the property is valid.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="form">The form instance.</param>
    /// <param name="propertyName">Name of the property to check.</param>
    /// <returns>The first validation error message, or null if valid.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> is null.</exception>
    public static string? GetValidationError<TModel>(this Form<TModel> form, string propertyName) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        return form.ValidationErrors
            .Where(v => v.MemberNames.Contains(propertyName, StringComparer.Ordinal))
            .Select(v => v.ErrorMessage)
            .FirstOrDefault();
    }

    /// <summary>
    /// Gets all validation error messages for the specified property.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="form">The form instance.</param>
    /// <param name="propertyName">Name of the property to check.</param>
    /// <returns>Read-only list of validation error messages.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> is null.</exception>
    public static IReadOnlyList<string> GetValidationErrors<TModel>(this Form<TModel> form, string propertyName) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        return form.ValidationErrors
            .Where(v => v.MemberNames.Contains(propertyName, StringComparer.Ordinal))
            .Select(v => v.ErrorMessage)
            .ToList();
    }

    /// <summary>
    /// Gets the value of a property from the form's model as a string representation.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    /// <param name="form">The form instance.</param>
    /// <param name="propertyName">Name of the property to get.</param>
    /// <returns>The string representation of the property value, or null if the property doesn't exist or is null.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
    public static string? GetModelValueAsString<TModel, TValue>(this Form<TModel> form, string propertyName) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var property = typeof(TModel).GetProperty(propertyName);
        if (property?.GetValue(form.Model) is not TValue value)
        {
            return null;
        }

        return value switch
        {
            null => null,
            string str => str,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value?.ToString()
        };
    }

    /// <summary>
    /// Attempts to set a property value on the form's model from a string representation.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <typeparam name="TValue">The property type.</typeparam>
    /// <param name="form">The form instance.</param>
    /// <param name="propertyName">Name of the property to set.</param>
    /// <param name="value">The string value to parse.</param>
    /// <returns>True if the value was successfully parsed and set; false otherwise.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="form"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="propertyName"/> is null or empty.</exception>
    public static bool TrySetModelValueFromString<TModel, TValue>(this Form<TModel> form, string propertyName, string? value) where TModel : new()
    {
        ArgumentNullException.ThrowIfNull(form);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        var property = typeof(TModel).GetProperty(propertyName);
        if (property?.PropertyType != typeof(TValue))
        {
            return false;
        }

        try
        {
            object? parsedValue = value switch
            {
                null => default(TValue),
                string str when typeof(TValue) == typeof(string) => str,
                string str when typeof(TValue) == typeof(int) => int.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(double) => double.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(decimal) => decimal.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(bool) => bool.Parse(str),
                string str when typeof(TValue) == typeof(DateTime) => DateTime.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(DateTimeOffset) => DateTimeOffset.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(Guid) => Guid.Parse(str),
                string str when typeof(TValue) == typeof(float) => float.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(long) => long.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(short) => short.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(byte) => byte.Parse(str, CultureInfo.InvariantCulture),
                string str when typeof(TValue) == typeof(char) => str.Length > 0 ? str[0] : default(char),
                _ => null
            };

            if (parsedValue != null || typeof(TValue).IsValueType)
            {
                property.SetValue(form.Model, parsedValue);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}