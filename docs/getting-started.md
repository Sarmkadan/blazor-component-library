# Getting Started with Blazor Component Library

This guide will help you get up and running with the Blazor Component Library in minutes.

## Prerequisites

- **.NET 10 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com)
- **Visual Studio 2022** or **Visual Studio Code** with C# extension
- **Git** for cloning the repository

## Installation

### Option 1: Use as NuGet Package

When the library is published to NuGet:

```bash
dotnet add package BlazorComponentLibrary
```

### Option 2: Project Reference

Clone and reference the library locally:

```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
cd blazor-component-library
dotnet build
```

In your Blazor project `.csproj`:

```xml
<ItemGroup>
    <ProjectReference Include="../blazor-component-library/BlazorComponentLibrary.csproj" />
</ItemGroup>
```

### Option 3: Docker

Quick demo without local setup:

```bash
docker-compose up
```

This builds and runs the demo application at `http://localhost:5000`.

## Basic Setup

### 1. Create a New Blazor Web App

```bash
dotnet new blazor -n MyBlazorApp
cd MyBlazorApp
dotnet add reference ../blazor-component-library/BlazorComponentLibrary.csproj
```

### 2. Configure Services

In `Program.cs`:

```csharp
using BlazorComponentLibrary.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add Blazor support
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Blazor Component Library - this registers all services
builder.Services.AddBlazorComponentLibrary();

var app = builder.Build();

// Configure pipeline
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

### 3. Inject Services in Components

```csharp
@page "/example"
@using BlazorComponentLibrary.Models
@using BlazorComponentLibrary.Services
@inject ComponentService ComponentService
@inject DataService DataService

<PageTitle>Example</PageTitle>

<h1>My First Component</h1>

@if (components != null)
{
    <ul>
        @foreach (var component in components)
        {
            <li>@component.Name</li>
        }
    </ul>
}

@code {
    private List<ComponentConfig>? components;

    protected override async Task OnInitializedAsync()
    {
        components = (await ComponentService.GetAllComponentsAsync())
            .ToList();
    }
}
```

## Common Tasks

### Create a Data Table

```csharp
@page "/data-table"
@inject ComponentService ComponentService
@inject DataService DataService

<h2>User Table</h2>

@if (rows != null)
{
    <table>
        <thead>
            <tr>
                <th>Name</th>
                <th>Email</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var row in rows)
            {
                <tr>
                    <td>@row.Values["Name"]</td>
                    <td>@row.Values["Email"]</td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<DataTableRow>? rows;

    protected override async Task OnInitializedAsync()
    {
        // Create component configuration
        var config = new ComponentConfig
        {
            Name = "UserTable",
            ComponentType = "DataTable",
            Description = "List of users"
        };

        await ComponentService.CreateComponentAsync(config);

        // Load table data
        rows = await DataService.GetTableDataAsync("users");
    }
}
```

### Build a Form

```csharp
@page "/form-example"
@inject FormService FormService

<EditForm Model="@Model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label>Email Address:</label>
        <InputText @bind-Value="Model.Email" class="form-control" />
    </div>

    <div class="form-group">
        <label>Message:</label>
        <InputTextArea @bind-Value="Model.Message" class="form-control" rows="4" />
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
</EditForm>

@code {
    public class FormModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Message { get; set; } = "";
    }

    private FormModel Model = new();

    private async Task HandleSubmit()
    {
        // Process form submission
        Console.WriteLine($"Email: {Model.Email}");
        await FormService.CreateFieldAsync(new FormField
        {
            Name = "submitted_form",
            Label = "Submitted form"
        });
    }
}
```

### Display a Chart

```csharp
@page "/chart-example"
@inject DataService DataService

<h2>Sales Chart</h2>

@if (dataset != null)
{
    <div style="width: 500px; height: 300px;">
        <!-- In a real app, integrate with a charting library like Chart.js -->
        <p>Chart displaying: @dataset.Label</p>
        <p>Data points: @dataset.Data.Count</p>
    </div>
}

@code {
    private ChartDataset? dataset;

    protected override Task OnInitializedAsync()
    {
        dataset = new ChartDataset
        {
            Label = "Monthly Sales",
            Data = new List<decimal> { 1000m, 1500m, 1200m, 2000m },
            ChartType = ChartType.Line,
            BackgroundColor = "rgba(75, 192, 192, 0.2)",
            BorderColor = "rgba(75, 192, 192, 1)"
        };

        return Task.CompletedTask;
    }
}
```

### User Authentication

```csharp
@page "/login"
@using System.ComponentModel.DataAnnotations
@inject UserService UserService
@inject NavigationManager NavigationManager

<div class="login-card">
    <h2>Sign In</h2>

    @if (!string.IsNullOrEmpty(ErrorMessage))
    {
        <div class="alert alert-danger">@ErrorMessage</div>
    }

    <EditForm Model="@LoginModel" OnValidSubmit="@HandleLogin">
        <DataAnnotationsValidator />

        <div class="form-group">
            <label>Username:</label>
            <InputText @bind-Value="LoginModel.Username" class="form-control" />
            <ValidationMessage For="@(() => LoginModel.Username)" />
        </div>

        <div class="form-group">
            <label>Password:</label>
            <InputText @bind-Value="LoginModel.Password" type="password" class="form-control" />
            <ValidationMessage For="@(() => LoginModel.Password)" />
        </div>

        <button type="submit" class="btn btn-primary">Login</button>
    </EditForm>
</div>

@code {
    public class LoginInput
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";
    }

    private LoginInput LoginModel = new();
    private string? ErrorMessage;

    private async Task HandleLogin()
    {
        try
        {
            var user = await UserService.AuthenticateAsync(
                LoginModel.Username,
                LoginModel.Password
            );

            if (user != null)
            {
                NavigationManager.NavigateTo("/dashboard");
            }
            else
            {
                ErrorMessage = "Invalid credentials";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
    }
}
```

## Next Steps

- Read the [API Reference](api-reference.md) for complete method documentation
- Explore [Architecture](architecture.md) for design patterns
- Check [Configuration](../docs/configuration.md) for advanced setup
- Review [examples/](../examples/) for complete working applications
- Consult [FAQ](faq.md) for common questions

## Getting Help

- **Documentation:** https://sarmkadan.com
- **GitHub Issues:** Report bugs or request features
- **Discussions:** Ask questions in GitHub Discussions

## Troubleshooting

### "Services not found" error
Ensure `AddBlazorComponentLibrary()` is called in `Program.cs` before `Build()`.

### "NavigationManager not found"
Inject it in your component: `@inject NavigationManager NavigationManager`

### "Database connection error"
The library uses in-memory storage by default. To use a database, implement custom repositories.

---

Ready to build something amazing? Start with the [examples](../examples/) directory for complete working code!
