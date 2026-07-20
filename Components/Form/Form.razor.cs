namespace BlazorComponentLibrary.Components.Form;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

/// <summary>
/// A form component that binds a model instance and validates it with
/// <see cref="System.ComponentModel.DataAnnotations"/> attributes on submit.
/// </summary>
/// <typeparam name="TModel">The model type bound to the form.</typeparam>
public sealed partial class Form<TModel> : ComponentBase, IForm<TModel> where TModel : new()
{
    private TModel _model = new();
    private IReadOnlyList<ValidationResult> _validationErrors = Array.Empty<ValidationResult>();
    private bool _isDirty;
    private EditContext? _editContext;

    /// <summary>Gets or sets the content rendered inside the form element.</summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>Event callback invoked with the model when the form is submitted and valid.</summary>
    [Parameter]
    public EventCallback<TModel> OnSubmit { get; set; }

    /// <summary>
    /// Gets the current data model bound to the form.
    /// </summary>
    public TModel Model => _model;

    /// <summary>
    /// Gets a value indicating whether the most recent call to <see cref="Validate"/>
    /// found no errors. Returns <c>true</c> if validation has not run yet.
    /// </summary>
    public bool IsValid => _validationErrors.Count == 0;

    /// <summary>
    /// Gets the validation errors produced by the most recent call to <see cref="Validate"/>.
    /// Empty if validation has not run yet or the model is valid.
    /// </summary>
    public IReadOnlyList<ValidationResult> ValidationErrors => _validationErrors;

    /// <summary>
    /// Gets a value indicating whether any field in the form has been modified.
    /// </summary>
    public bool IsDirty => _isDirty;

    /// <summary>
    /// Event callback invoked when any field in the form is modified.
    /// </summary>
    [Parameter]
    public EventCallback FieldsChanged { get; set; }

    /// <summary>
    /// Sets the data model for the form.
    /// </summary>
    /// <param name="model">The model instance to bind to the form.</param>
    public void SetModel(TModel model)
    {
        _model = model ?? new TModel();
        _validationErrors = Array.Empty<ValidationResult>();
        _isDirty = false;
        _editContext = null;
        NotifyStateChanged(); // Notify Blazor that the component state has changed
    }

    /// <summary>
    /// Notifies the component that its state has changed.
    /// </summary>
    private void NotifyStateChanged()
    {
        try
        {
            StateHasChanged();
        }
        catch (InvalidOperationException)
        {
            // Ignore if the component is not attached to a renderer (e.g. unit tests).
        }
    }

    /// <summary>
    /// Handles field changes and marks the form as dirty.
    /// </summary>
    private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
    {
        _isDirty = true;
        FieldsChanged.InvokeAsync();
        NotifyStateChanged();
    }

    /// <summary>
    /// Overrides the base method to initialize the EditContext for field change tracking.
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (_editContext == null || !object.Equals(_model, _editContext.Model))
        {
            _editContext = new EditContext(_model);
            _editContext.OnFieldChanged += HandleFieldChanged;
        }
    }

    /// <summary>
    /// Validates the current model against its
    /// <see cref="System.ComponentModel.DataAnnotations"/> attributes,
    /// including <see cref="IValidatableObject"/> implementations.
    /// The results are exposed via <see cref="ValidationErrors"/> and <see cref="IsValid"/>.
    /// </summary>
    /// <returns>True if the model is valid, false otherwise.</returns>
    public Task<bool> Validate()
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(_model!);
        var isValid = Validator.TryValidateObject(_model!, context, results, validateAllProperties: true);

        _validationErrors = results;
        NotifyStateChanged();
        return Task.FromResult(isValid);
    }

    /// <summary>
    /// Handles form submission: validates the model and invokes
    /// <see cref="OnSubmit"/> only when validation succeeds.
    /// </summary>
    protected async Task HandleSubmit()
    {
        if (await Validate())
        {
            _isDirty = false;
            await OnSubmit.InvokeAsync(_model);
        }
    }

    /// <summary>
    /// Resets the form to its initial state, clearing any modifications.
    /// </summary>
    public void Reset()
    {
        _isDirty = false;
        NotifyStateChanged();
    }
}
