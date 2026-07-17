namespace BlazorComponentLibrary.Components.Form;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

/// <summary>
/// Provides validation helpers for <see cref="Form{TModel}"/> components.
/// </summary>
public static class FormValidation
{
	/// <summary>
	/// Validates the form and returns a list of human-readable validation problems.
	/// </summary>
	/// <typeparam name="TModel">The model type bound to the form.</typeparam>
	/// <param name="form">The form to validate.</param>
	/// <returns>An immutable list of validation error messages. Empty if the form is valid.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="form"/> is null.</exception>
	public static IReadOnlyList<string> Validate<TModel>(this Form<TModel> form) where TModel : new()
	{
		ArgumentNullException.ThrowIfNull(form);

		var model = form.Model;
		var results = new List<ValidationResult>();
		var context = new ValidationContext(model ?? new TModel());
		var isValid = Validator.TryValidateObject(model ?? new TModel(), context, results, validateAllProperties: true);

		if (isValid)
		{
			return Array.Empty<string>();
		}

		var errorMessages = new List<string>(results.Count);
		foreach (var validationResult in results)
		{
			if (validationResult.ErrorMessage is { Length: > 0 } errorMessage)
			{
				errorMessages.Add(errorMessage);
			}
		}

		return errorMessages;
	}

	/// <summary>
	/// Determines whether the form is valid.
	/// </summary>
	/// <typeparam name="TModel">The model type bound to the form.</typeparam>
	/// <param name="form">The form to validate.</param>
	/// <returns>True if the form is valid; otherwise, false.</returns>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="form"/> is null.</exception>
	public static bool IsValid<TModel>(this Form<TModel> form) where TModel : new()
	{
		ArgumentNullException.ThrowIfNull(form);
		return form.IsValid;
	}

	/// <summary>
	/// Validates the form and throws an exception if it is invalid.
	/// </summary>
	/// <typeparam name="TModel">The model type bound to the form.</typeparam>
	/// <param name="form">The form to validate.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="form"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when the form is invalid, containing a list of validation problems.</exception>
	public static void EnsureValid<TModel>(this Form<TModel> form) where TModel : new()
	{
		ArgumentNullException.ThrowIfNull(form);

		var errors = FormValidation.Validate(form);
		if (errors.Count == 0)
		{
			return;
		}

		throw new ArgumentException(
			$"Form is invalid. Problems:\n{string.Join("\n", errors)}",
			nameof(form));
	}
}
