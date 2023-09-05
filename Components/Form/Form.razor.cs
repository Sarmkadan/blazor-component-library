namespace BlazorComponentLibrary.Components.Form;

using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

public partial class Form<TModel> : ComponentBase, IForm<TModel> where TModel : new()
{
    private TModel _model = new TModel();

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public EventCallback<TModel> OnSubmit { get; set; }

    /// <summary>
    /// Gets the current data model bound to the form.
    /// </summary>
    public TModel Model => _model;

    /// <summary>
    /// Gets a value indicating whether the form's current state is valid.
    /// (Placeholder: always returns true for now)
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Sets the data model for the form.
    /// </summary>
    /// <param name="model">The model instance to bind to the form.</param>
    public void SetModel(TModel model)
    {
        _model = model ?? new TModel();
        StateHasChanged(); // Notify Blazor that the component state has changed
    }

    /// <summary>
    /// Triggers validation for all fields in the form.
    /// (Placeholder: always returns true for now)
    /// </summary>
    /// <returns>True if the form is valid, false otherwise.</returns>
    public Task<bool> Validate()
    {
        // For now, always return true. Real validation logic would go here.
        return Task.FromResult(true);
    }

    protected async Task HandleSubmit()
    {
        if (IsValid) // In a real scenario, Validate() would be called here.
        {
            await OnSubmit.InvokeAsync(_model);
        }
    }
}
