# Blazor Component Library Examples

Complete working examples demonstrating how to use the Blazor Component Library.

## Available Examples

### 1. Basic Data Table (`01-BasicDataTable.razor`)

**Demonstrates:**
- Creating a data table with sample data
- Loading and displaying table rows
- Exporting data to CSV and JSON formats
- Pagination support

**Key Features:**
- Sample user data with departments and status
- Export functionality
- Error handling
- Responsive design

**Use Case:** Displaying and managing tabular data

---

### 2. Form Validation (`02-FormValidation.razor`)

**Demonstrates:**
- Creating strongly-typed forms
- Input field validation
- Custom validation messages
- Form submission handling

**Key Features:**
- Multiple field types (text, email, textarea, select, checkbox)
- Data annotations for validation
- Real-time validation feedback
- Form reset functionality

**Use Case:** User data collection with validation

---

### 3. User Authentication (`03-UserAuthentication.razor`)

**Demonstrates:**
- User login functionality
- Account registration
- Role-based access control
- User management and permissions

**Key Features:**
- Login and registration tabs
- User dashboard
- Role assignment (Viewer, Editor, Admin)
- Session management

**Use Case:** Securing your application with user authentication

---

### 4. Chart Data (`04-ChartData.razor`)

**Demonstrates:**
- Creating datasets for different chart types
- Working with chart models
- Visualizing data
- Exporting chart data

**Key Features:**
- Support for 8 chart types
- Dynamic data generation
- Color customization
- Data export capability

**Use Case:** Data visualization and reporting

---

### 5. Theme Management (`05-ThemeManagement.razor`)

**Demonstrates:**
- Creating and applying themes
- Managing color schemes
- CSS variable generation
- Custom theme creation

**Key Features:**
- Predefined theme templates
- Custom theme builder
- Color picker interface
- Dark/Light mode support

**Use Case:** Customizing application appearance

---

### 6. Advanced Data Operations (`06-AdvancedDataOperations.razor`)

**Demonstrates:**
- Pagination with multiple page controls
- Filtering by multiple criteria
- Sorting in ascending/descending order
- Data aggregation and statistics

**Key Features:**
- Multi-field filtering
- Dynamic pagination
- Sorting by different columns
- Statistics calculation
- Results summary

**Use Case:** Complex data management and analysis

---

## Running the Examples

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 or Visual Studio Code
- Blazor Web App project

### Setup

1. **Create a new Blazor project** (if you don't have one)
   ```bash
   dotnet new blazor -n BlazorApp
   cd BlazorApp
   ```

2. **Add project reference**
   ```bash
   dotnet add reference ../blazor-component-library/BlazorComponentLibrary.csproj
   ```

3. **Copy example files**
   ```bash
   cp ../blazor-component-library/examples/*.razor Pages/
   ```

4. **Update Program.cs**
   ```csharp
   using BlazorComponentLibrary.Configuration;
   
   builder.Services.AddBlazorComponentLibrary();
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Navigate to examples**
   - http://localhost:5000/examples/basic-data-table
   - http://localhost:5000/examples/form-validation
   - http://localhost:5000/examples/user-authentication
   - http://localhost:5000/examples/chart-data
   - http://localhost:5000/examples/theme-management
   - http://localhost:5000/examples/advanced-data

## Example Components

Each example includes:

- **Complete working code** - Copy and use as-is
- **Comments and documentation** - Inline explanations
- **Error handling** - Proper exception handling
- **Styling** - Scoped CSS with responsive design
- **Best practices** - Following C# and Blazor conventions

## Customizing Examples

### Modify Data

```csharp
// Change the sample data in OnInitializedAsync
var sampleData = new[]
{
    // Your data here
};
```

### Customize Styling

```csharp
<style>
    /* Add your custom styles */
    .my-class {
        color: blue;
    }
</style>
```

### Extend Functionality

```csharp
@code {
    // Add your own methods and properties
    private async Task MyCustomMethod()
    {
        // Implementation
    }
}
```

## Integration with Your Application

### Import and Use

```csharp
@page "/my-page"
@using BlazorComponentLibrary.Models
@using BlazorComponentLibrary.Services
@inject DataService DataService

<!-- Your component content -->
```

### Service Injection

```csharp
@code {
    [Inject]
    public ComponentService ComponentService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        // Use services
        var components = await ComponentService.GetAllComponentsAsync();
    }
}
```

## Testing Your Changes

1. **Build the project**
   ```bash
   dotnet build
   ```

2. **Run tests** (if available)
   ```bash
   dotnet test
   ```

3. **Test in browser**
   - Navigate to example pages
   - Test all functionality
   - Check browser console for errors

## Troubleshooting

### Service Not Found Error

**Problem:** `InvalidOperationException: No service for type...`

**Solution:** Ensure `AddBlazorComponentLibrary()` is called in `Program.cs`

### Component Not Rendering

**Problem:** Page is blank or shows error

**Solution:** 
- Check browser console for errors
- Verify services are properly injected
- Check `OnInitializedAsync` implementation

### Data Not Loading

**Problem:** Empty tables or forms

**Solution:**
- Verify sample data is generated
- Check async operations complete
- Call `StateHasChanged()` if needed

### Styling Issues

**Problem:** Components don't look right

**Solution:**
- Clear browser cache
- Check CSS selector specificity
- Ensure styles are properly scoped

## Best Practices

1. **Always use async/await** for service calls
2. **Handle exceptions** in try-catch blocks
3. **Call StateHasChanged()** when needed
4. **Use type-safe models** instead of dynamic objects
5. **Inject dependencies** via `@inject` or `[Inject]`
6. **Test in multiple browsers** for compatibility

## Learning Resources

- [Getting Started Guide](../docs/getting-started.md)
- [API Reference](../docs/api-reference.md)
- [Architecture Documentation](../docs/architecture.md)
- [FAQ](../docs/faq.md)
- [Main README](../README.md)

## Creating Your Own Examples

To add a new example:

1. Create a new `.razor` file in `examples/` directory
2. Add page route: `@page "/examples/your-example"`
3. Include the author header comment
4. Add comprehensive comments
5. Include scoped CSS styling
6. Update this README with description

**Template:**
```csharp
@* =============================================================================
 * Author: Vladyslav Zaiets | https://sarmkadan.com
 * CTO & Software Architect
 * =============================================================================
 *
 * Your Example Title
 * Demonstrates: Feature 1, Feature 2, Feature 3
 *@

@page "/examples/your-example"
@using BlazorComponentLibrary.Services
@inject YourService YourService

<h1>Your Example</h1>

<!-- Your content -->

@code {
    // Your code
}

<style>
    /* Your styles */
</style>
```

## Contributing

Have a great example to share? Contribute it!

1. Follow the template above
2. Include clear documentation
3. Add error handling
4. Test thoroughly
5. Submit a pull request

---

**Need help?** Check the [FAQ](../docs/faq.md) or visit https://sarmkadan.com
