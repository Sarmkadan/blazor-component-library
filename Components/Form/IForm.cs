namespace BlazorComponentLibrary.Components.Form;

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public interface IForm<TModel>
{
    /// <summary>
    /// Sets the data model for the form.
    /// </summary>
    /// <param name="model">The model instance to bind to the form.</param>
    void SetModel(TModel model);

    /// <summary>
    /// Gets the current data model bound to the form.
    /// </summary>
    TModel Model { get; }

    /// <summary>
    /// Event callback for when the form is submitted.
    /// </summary>
    EventCallback<TModel> OnSubmit { get; set; }

    /// <summary>
    /// Gets a value indicating whether the form's current state is valid.
    /// </summary>
    bool IsValid { get; }

    /// <summary>
    /// Gets a value indicating whether any field in the form has been modified.
    /// </summary>
    bool IsDirty { get; }

    /// <summary>
    /// Event callback invoked when any field in the form is modified.
    /// </summary>
    EventCallback FieldsChanged { get; set; }

    /// <summary>
    /// Triggers validation for all fields in the form.
    /// </summary>
    /// <returns>True if the form is valid, false otherwise.</returns>
    Task<bool> Validate();

    /// <summary>
    /// Resets the form to its initial state, clearing any modifications and validation state.
    /// </summary>
    void Reset();
}
