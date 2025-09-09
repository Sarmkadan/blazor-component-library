# API Reference

Complete reference for all public types, methods, and services in the Blazor Component Library.

## Services

### ComponentService

Manages component configurations and lifecycle.

**Namespace:** `BlazorComponentLibrary.Services`

#### Methods

##### CreateComponentAsync
```csharp
public async Task<ComponentConfig> CreateComponentAsync(ComponentConfig config)
```
Creates a new component configuration.

**Parameters:**
- `config` (ComponentConfig): Component configuration to create

**Returns:** Created ComponentConfig with assigned ID

**Throws:**
- `ArgumentNullException` - If config is null
- `ArgumentException` - If Name is empty
- `ConflictException` - If component name already exists

**Example:**
```csharp
var config = new ComponentConfig
{
    Name = "UserTable",
    ComponentType = "DataTable",
    Description = "Users data table"
};
var created = await componentService.CreateComponentAsync(config);
```

##### GetComponentByIdAsync
```csharp
public async Task<ComponentConfig?> GetComponentByIdAsync(int id)
```
Retrieves a component by ID.

**Parameters:**
- `id` (int): Component ID

**Returns:** ComponentConfig if found; null otherwise

**Example:**
```csharp
var component = await componentService.GetComponentByIdAsync(1);
if (component != null)
{
    Console.WriteLine(component.Name);
}
```

##### GetAllComponentsAsync
```csharp
public async Task<IEnumerable<ComponentConfig>> GetAllComponentsAsync()
```
Retrieves all components.

**Returns:** Enumerable of all ComponentConfigs

**Example:**
```csharp
var components = await componentService.GetAllComponentsAsync();
foreach (var comp in components)
{
    Console.WriteLine(comp.Name);
}
```

##### SearchComponentsAsync
```csharp
public async Task<IEnumerable<ComponentConfig>> SearchComponentsAsync(string query)
```
Searches components by name or description.

**Parameters:**
- `query` (string): Search term

**Returns:** Matching ComponentConfigs

**Example:**
```csharp
var results = await componentService.SearchComponentsAsync("table");
```

##### UpdateComponentAsync
```csharp
public async Task UpdateComponentAsync(int id, ComponentConfig config)
```
Updates an existing component.

**Parameters:**
- `id` (int): Component ID
- `config` (ComponentConfig): Updated configuration

**Throws:**
- `ComponentNotFoundException` - If component not found

**Example:**
```csharp
var updated = new ComponentConfig
{
    Name = "UserTable-Updated",
    ComponentType = "DataTable"
};
await componentService.UpdateComponentAsync(1, updated);
```

##### DeleteComponentAsync
```csharp
public async Task DeleteComponentAsync(int id)
```
Deletes a component.

**Parameters:**
- `id` (int): Component ID

**Throws:**
- `ComponentNotFoundException` - If component not found

##### GetComponentStatisticsAsync
```csharp
public async Task<ComponentStatistics> GetComponentStatisticsAsync()
```
Returns statistics about components.

**Returns:** ComponentStatistics with counts and types

---

### DataService

Manages table and chart data.

**Namespace:** `BlazorComponentLibrary.Services`

#### Methods

##### AddRowAsync
```csharp
public async Task AddRowAsync(string tableName, DataTableRow row)
```
Adds a row to a table.

**Parameters:**
- `tableName` (string): Table identifier
- `row` (DataTableRow): Row data

**Example:**
```csharp
var row = new DataTableRow
{
    Values = new Dictionary<string, object>
    {
        { "Id", 1 },
        { "Name", "John Doe" },
        { "Email", "john@example.com" }
    }
};
await dataService.AddRowAsync("users", row);
```

##### GetTableDataAsync
```csharp
public async Task<List<DataTableRow>> GetTableDataAsync(string tableName)
```
Retrieves all rows from a table.

**Parameters:**
- `tableName` (string): Table identifier

**Returns:** List of DataTableRows

##### GetPagedDataAsync
```csharp
public async Task<PaginatedResult<DataTableRow>> GetPagedDataAsync(
    string tableName, 
    int pageNumber, 
    int pageSize)
```
Retrieves paginated table data.

**Parameters:**
- `tableName` (string): Table identifier
- `pageNumber` (int): Page number (1-based)
- `pageSize` (int): Items per page

**Returns:** PaginatedResult with rows and pagination info

**Example:**
```csharp
var page = await dataService.GetPagedDataAsync("users", 1, 25);
Console.WriteLine($"Total: {page.TotalCount}");
foreach (var row in page.Items)
{
    Console.WriteLine(row.Values["Name"]);
}
```

##### ExportToFormatAsync
```csharp
public async Task<string> ExportToFormatAsync(
    string tableName, 
    ExportFormat format)
```
Exports table data to a specific format.

**Parameters:**
- `tableName` (string): Table identifier
- `format` (ExportFormat): CSV, JSON, or XML

**Returns:** Formatted string representation of data

**Example:**
```csharp
var csv = await dataService.ExportToFormatAsync("users", ExportFormat.Csv);
// csv contains: Name,Email,Status\nJohn,john@example.com,Active\n...
```

##### UpdateRowAsync
```csharp
public async Task UpdateRowAsync(
    string tableName, 
    int rowId, 
    DataTableRow row)
```
Updates an existing row.

**Parameters:**
- `tableName` (string): Table identifier
- `rowId` (int): Row identifier
- `row` (DataTableRow): Updated row data

##### DeleteRowAsync
```csharp
public async Task DeleteRowAsync(string tableName, int rowId)
```
Deletes a row from a table.

**Parameters:**
- `tableName` (string): Table identifier
- `rowId` (int): Row identifier

---

### FormService

Manages form fields and validation.

**Namespace:** `BlazorComponentLibrary.Services`

#### Methods

##### CreateFieldAsync
```csharp
public async Task<FormField> CreateFieldAsync(FormField field)
```
Creates a new form field.

**Parameters:**
- `field` (FormField): Field configuration

**Returns:** Created FormField

**Example:**
```csharp
var field = new FormField
{
    Name = "email",
    Label = "Email Address",
    FieldType = FormFieldType.Email,
    IsRequired = true
};
await formService.CreateFieldAsync(field);
```

##### ValidateFormAsync
```csharp
public async Task<FormValidationResult> ValidateFormAsync(
    Dictionary<string, object> formData)
```
Validates form data against configured fields.

**Parameters:**
- `formData` (Dictionary<string, object>): Form data to validate

**Returns:** FormValidationResult with IsValid flag and errors

**Example:**
```csharp
var data = new Dictionary<string, object>
{
    { "email", "user@example.com" },
    { "name", "John" }
};

var result = await formService.ValidateFormAsync(data);
if (result.IsValid)
{
    // Process form
}
else
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Key}: {error.Value}");
    }
}
```

##### GetAllFieldsAsync
```csharp
public async Task<IEnumerable<FormField>> GetAllFieldsAsync()
```
Retrieves all form fields.

**Returns:** Enumerable of FormFields

##### UpdateFieldAsync
```csharp
public async Task UpdateFieldAsync(string name, FormField field)
```
Updates a form field configuration.

**Parameters:**
- `name` (string): Field name
- `field` (FormField): Updated configuration

##### DeleteFieldAsync
```csharp
public async Task DeleteFieldAsync(string name)
```
Deletes a form field.

**Parameters:**
- `name` (string): Field name to delete

---

### UserService

Manages users, authentication, and authorization.

**Namespace:** `BlazorComponentLibrary.Services`

#### Methods

##### CreateUserAsync
```csharp
public async Task<User> CreateUserAsync(
    string username, 
    string email, 
    string password)
```
Creates a new user account.

**Parameters:**
- `username` (string): Unique username
- `email` (string): User email
- `password` (string): User password

**Returns:** Created User

**Throws:**
- `ArgumentException` - If username/email format invalid
- `ConflictException` - If username already exists

**Example:**
```csharp
var user = await userService.CreateUserAsync(
    username: "john_doe",
    email: "john@example.com",
    password: "SecurePassword123!"
);
```

##### AuthenticateAsync
```csharp
public async Task<User?> AuthenticateAsync(
    string username, 
    string password)
```
Authenticates a user.

**Parameters:**
- `username` (string): Username
- `password` (string): Password

**Returns:** User if authenticated; null otherwise

**Example:**
```csharp
var user = await userService.AuthenticateAsync("john_doe", "SecurePassword123!");
if (user != null)
{
    // User authenticated
}
```

##### GetUserByIdAsync
```csharp
public async Task<User?> GetUserByIdAsync(int id)
```
Retrieves a user by ID.

**Parameters:**
- `id` (int): User ID

**Returns:** User if found; null otherwise

##### UpdateRoleAsync
```csharp
public async Task UpdateRoleAsync(int userId, UserRole role)
```
Updates user role.

**Parameters:**
- `userId` (int): User ID
- `role` (UserRole): New role (Admin, Editor, Viewer)

##### HasPermissionAsync
```csharp
public async Task<bool> HasPermissionAsync(
    int userId, 
    string permission)
```
Checks if user has specific permission.

**Parameters:**
- `userId` (int): User ID
- `permission` (string): Permission name

**Returns:** true if user has permission; false otherwise

**Example:**
```csharp
bool canDelete = await userService.HasPermissionAsync(userId, "CanDeleteUsers");
```

##### GetAllUsersAsync
```csharp
public async Task<IEnumerable<User>> GetAllUsersAsync()
```
Retrieves all users.

**Returns:** Enumerable of all Users

##### DeleteUserAsync
```csharp
public async Task DeleteUserAsync(int id)
```
Deletes a user account.

**Parameters:**
- `id` (int): User ID

---

### ThemeService

Manages application themes and styling.

**Namespace:** `BlazorComponentLibrary.Services`

#### Methods

##### CreateThemeAsync
```csharp
public async Task<Theme> CreateThemeAsync(Theme theme)
```
Creates a new theme.

**Parameters:**
- `theme` (Theme): Theme configuration

**Returns:** Created Theme

##### GetCurrentThemeAsync
```csharp
public async Task<Theme?> GetCurrentThemeAsync()
```
Retrieves the currently active theme.

**Returns:** Current Theme; null if not set

##### SetActiveThemeAsync
```csharp
public async Task SetActiveThemeAsync(string themeName)
```
Sets the active theme.

**Parameters:**
- `themeName` (string): Name of theme to activate

##### GenerateCssVariablesAsync
```csharp
public async Task<string> GenerateCssVariablesAsync()
```
Generates CSS custom properties for current theme.

**Returns:** CSS custom properties string

**Example:**
```csharp
var css = await themeService.GenerateCssVariablesAsync();
// Returns: :root { --primary: #007bff; --secondary: #6c757d; ... }
```

---

## Models

### ComponentConfig

Represents a component configuration.

**Properties:**
- `Id` (int): Component identifier
- `Name` (string): Component name
- `ComponentType` (string): Type (DataTable, Chart, Form, Modal)
- `Description` (string): Component description
- `CreatedAt` (DateTime): Creation timestamp
- `UpdatedAt` (DateTime): Last update timestamp

### DataTableColumn

Represents a table column.

**Properties:**
- `Name` (string): Column name
- `DisplayName` (string): Display label
- `SortOrder` (SortOrder?): Sort order (Ascending, Descending, None)
- `IsVisible` (bool): Visibility flag
- `Width` (int?): Column width in pixels
- `Sortable` (bool): Whether column is sortable

### DataTableRow

Represents a table row.

**Properties:**
- `Id` (int): Row identifier
- `Values` (Dictionary<string, object>): Column values
- `IsSelected` (bool): Selection flag

### ChartDataset

Represents chart data.

**Properties:**
- `Label` (string): Dataset label
- `Data` (List<decimal>): Data points
- `ChartType` (ChartType): Type (Line, Bar, Pie, etc.)
- `BackgroundColor` (string): Background color
- `BorderColor` (string): Border color
- `BorderWidth` (int): Border width

### FormField

Represents a form field.

**Properties:**
- `Name` (string): Field identifier
- `Label` (string): Field label
- `FieldType` (FormFieldType): Type (Text, Email, Password, etc.)
- `IsRequired` (bool): Required flag
- `ValidationRules` (Dictionary<string, string>): Validation rules
- `Placeholder` (string): Placeholder text
- `DefaultValue` (object?): Default value

### User

Represents a user account.

**Properties:**
- `Id` (int): User identifier
- `Username` (string): Unique username
- `Email` (string): Email address
- `Role` (UserRole): User role (Admin, Editor, Viewer)
- `CreatedAt` (DateTime): Account creation date
- `LastLogin` (DateTime?): Last login timestamp
- `IsActive` (bool): Account status

### Theme

Represents a UI theme.

**Properties:**
- `Name` (string): Theme name
- `IsDarkMode` (bool): Dark mode flag
- `PrimaryColor` (string): Primary color (hex)
- `SecondaryColor` (string): Secondary color (hex)
- `AccentColor` (string): Accent color (hex)
- `BackgroundColor` (string): Background color (hex)
- `TextColor` (string): Text color (hex)

---

## Enumerations

### FormFieldType
```csharp
public enum FormFieldType
{
    Text,
    Email,
    Password,
    Number,
    Checkbox,
    Radio,
    Select,
    TextArea,
    Date,
    Time,
    DateTime
}
```

### UserRole
```csharp
public enum UserRole
{
    Viewer = 0,
    Editor = 1,
    Admin = 2
}
```

### ChartType
```csharp
public enum ChartType
{
    Line,
    Bar,
    Pie,
    Doughnut,
    Area,
    Scatter,
    Bubble,
    Radar
}
```

### ExportFormat
```csharp
public enum ExportFormat
{
    Csv,
    Json,
    Xml
}
```

---

## Exceptions

### ComponentLibraryException
Base exception for the library.

```csharp
public class ComponentLibraryException : Exception
{
    public ComponentLibraryException(string message);
    public ComponentLibraryException(string message, Exception innerException);
}
```

### ComponentNotFoundException
Thrown when a component is not found.

### InvalidComponentException
Thrown when component configuration is invalid.

### FormValidationException
Thrown when form validation fails.

### UnauthorizedException
Thrown when authentication is required.

### ForbiddenException
Thrown when insufficient permissions.

### ConflictException
Thrown when resource conflict occurs.

---

## Extension Methods

### ServiceCollectionExtensions

```csharp
public static IServiceCollection AddBlazorComponentLibrary(
    this IServiceCollection services,
    Action<LibraryOptions>? configure = null)
```

Registers all services in the library.

**Example:**
```csharp
services.AddBlazorComponentLibrary(options =>
{
    options.EnableCaching = true;
    options.DefaultPageSize = 50;
});
```

---

For more examples and guidance, see [Getting Started](getting-started.md) and [Architecture](architecture.md).
