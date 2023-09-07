# BclForm

A generic form wrapper that binds a model, handles submission, and exposes validation state.

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | `RenderFragment` | — | Form fields and controls |
| `OnSubmit` | `EventCallback<TModel>` | — | Callback invoked with the bound model when the form is submitted and valid |

## Methods

| Method | Returns | Description |
|--------|---------|-------------|
| `SetModel(TModel)` | `void` | Replaces the bound model and triggers re-render |
| `Validate()` | `Task<bool>` | Runs validation on all fields; returns `true` if valid |
| `Model` | `TModel` (property) | Returns the current bound model instance |
| `IsValid` | `bool` (property) | Returns the current validation state |

## Basic usage

```razor
<BclForm TModel="ContactRequest" OnSubmit="@HandleSubmit" @ref="contactForm">
    <BclInput Label="Name"  @bind-Value="contactForm.Model.Name"  Required="true" />
    <BclInput Label="Email" @bind-Value="contactForm.Model.Email" Type="email" Required="true" />
    <BclTextArea Label="Message" @bind-Value="contactForm.Model.Message" Rows="4" />
    <BclButton Type="submit">Send message</BclButton>
</BclForm>

@code {
    private BclForm<ContactRequest> contactForm = default!;

    private async Task HandleSubmit(ContactRequest request)
    {
        await ApiClient.SendContactAsync(request);
    }
}
```

## Pre-populating a form for editing

```razor
<BclForm TModel="UserProfile" OnSubmit="@SaveProfile" @ref="profileForm">
    <BclInput Label="Display name" @bind-Value="profileForm.Model.DisplayName" />
    <BclButton Type="submit">Save</BclButton>
</BclForm>

@code {
    private BclForm<UserProfile> profileForm = default!;

    protected override async Task OnInitializedAsync()
    {
        var profile = await UserService.GetProfileAsync();
        profileForm.SetModel(profile);
    }

    private async Task SaveProfile(UserProfile profile) =>
        await UserService.UpdateProfileAsync(profile);
}
```

## Accessibility

- The `<BclForm>` renders a native `<form>` element; associate each label with its input using `for`/`id` pairs (or wrap the input inside the `<label>`).
- Required fields should have the `required` attribute and an explicit visual indicator (e.g. an asterisk with `aria-hidden="true"` and a legend explaining the convention).
- Display validation errors in a `role="alert"` region adjacent to the invalid field, or summarise them at the top of the form with `aria-live="polite"`.
- Successful submission should announce a confirmation to screen readers via a live region.

## Theming

```css
:root {
    --bcl-form-label-text:       #374151;
    --bcl-form-label-font-size:  0.875rem;
    --bcl-form-input-bg:         #ffffff;
    --bcl-form-input-border:     #d1d5db;
    --bcl-form-input-border-focus: #3b82f6;
    --bcl-form-input-radius:     0.375rem;
    --bcl-form-input-padding:    0.5rem 0.75rem;
    --bcl-form-error-text:       #ef4444;
    --bcl-form-gap:              1rem;
}
```
