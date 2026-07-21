# Form

`Form<TModel>` is a generic Blazor component designed to encapsulate form functionality with data binding, validation, and submission handling. It provides a structured way to manage form state and user input for models that implement a parameterless constructor, adhering to the `IForm<TModel>` interface for consistent integration within the `blazor-component-library`.

## API

### `ChildContent`
- **Type**: `RenderFragment?`
- **Purpose**: Defines the content rendered inside the form. This allows nesting of input components and other UI elements that contribute to the form's structure.
- **Parameters**: None.
- **Return Value**: N/A.
- **Exceptions**: None.

### `OnSubmit`
- **Type**: `EventCallback<TModel>`
- **Purpose**: Invoked when the form is successfully submitted. The callback receives the current model instance after validation.
- **Parameters**: `TModel` (the validated model instance).
- **Return Value**: N/A.
- **Exceptions**: None.

### `FieldsChanged`
- **Type**: `EventCallback`
- **Purpose**: Triggered whenever any field within the form is modified. Useful for real-time updates or tracking dirty states.
- **Parameters**: None.
- **Return Value**: N/A.
- **Exceptions**: None.

### `SetModel`
- **Type**: `void`
- **Purpose**: Assigns a new model instance to the form, replacing the existing one. Used to programmatically update the form's data context.
- **Parameters**: `TModel model` (the new model instance).
- **Return Value**: N/A.
- **Exceptions**: None.

### `Validate`
- **Type**: `Task<bool>`
- **Purpose**: Asynchronously validates all fields in the form. Returns `true` if validation passes, `false` otherwise.
- **Parameters**: None.
- **Return Value**: `Task<bool>` indicating validation success.
- **Exceptions**: None.

### `Reset`
- **Type**: `void`
- **Purpose**: Resets the form to its initial state, clearing all user input and restoring the model to its default values.
- **Parameters**: None.
- **Return Value**: N/A.
- **Exceptions**: None.

## Usage

### Basic Form with Submission Handling
```razor
<Form<TModel> OnSubmit="HandleSubmit">
    <InputText @bind-Value="context.Name" />
    <InputNumber @bind-Value="context.Age" />
    <button type="submit">Submit</button>
</Form>

@code {
    private void HandleSubmit(TModel model)
    {
        // Process submitted data
    }
}
```

### Form with Validation and Reset
```razor
<Form<TModel> @ref="form" OnSubmit="HandleSubmit" FieldsChanged="OnFieldsChanged">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <InputText @bind-Value="context.Email" />
    <button @onclick="() => form.Reset()">Reset</button>
    <button type="submit">Submit</button>
</Form>

@code {
    private Form<TModel> form;

    private async Task HandleSubmit(TModel model)
    {
        if (await form.Validate())
        {
            // Proceed with submission
        }
    }

    private void OnFieldsChanged()
    {
        // Handle field changes (e.g., enable save button)
    }
}
```

## Notes

- **Thread Safety**: Blazor components execute on the UI thread by default. `Form<TModel>` does not implement thread-safe mechanisms for concurrent access. Avoid modifying the form's model or invoking its methods from non-UI threads.
- **Model Initialization**: The `TModel` constraint (`where TModel : new`) ensures the form can instantiate a default model. However, `SetModel` may receive externally constructed instances, which should be handled cautiously to prevent unintended side effects.
- **Validation Timing**: `Validate` is asynchronous but does not block rendering. Multiple rapid calls may result in redundant validations; consider debouncing in high-frequency scenarios.
- **Reset Behavior**: `Reset` reinitializes the model using its parameterless constructor. If the model contains complex state or references, additional cleanup logic may be required outside the component.
