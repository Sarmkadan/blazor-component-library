# Blazor Component Library

A lightweight, production-grade Blazor component library featuring reusable UI components for data tables, charts, forms, and modals without heavy CSS framework dependencies.

**Author:** Vladyslav Zaiets  
**Website:** https://sarmkadan.com  
**License:** MIT (Copyright 2026)

## Overview

This library provides a comprehensive set of Blazor components and supporting services for building modern, responsive web applications. It focuses on:

- **Zero external CSS dependencies** - Minimal, composable styling
- **Type-safe APIs** - Full C# type safety with validation
- **Extensible architecture** - Easy to customize and extend
- **Production-ready** - Real error handling, logging, and patterns

## Project Structure

```
BlazorComponentLibrary/
├── Models/                          # Domain models (8 classes)
│   ├── ComponentConfig.cs          # Component configuration
│   ├── DataTableColumn.cs          # Table column definitions
│   ├── DataTableRow.cs             # Table row data
│   ├── ChartDataset.cs             # Chart data management
│   ├── FormField.cs                # Form field configuration
│   ├── ModalConfig.cs              # Modal settings
│   ├── Theme.cs                    # Theme management
│   ├── User.cs                     # User accounts and roles
│   └── Result.cs                   # Result wrapper types
├── Services/                        # Business logic layer (5 services)
│   ├── ComponentService.cs         # Component management
│   ├── DataService.cs              # Table and chart data
│   ├── FormService.cs              # Form validation and logic
│   ├── ThemeService.cs             # Theme management
│   └── UserService.cs              # User management & auth
├── Repositories/                    # Data access layer (5 repositories)
│   ├── IComponentRepository.cs
│   ├── ComponentRepository.cs
│   ├── IDataRepository.cs
│   ├── DataRepository.cs
│   ├── IFormRepository.cs
│   ├── FormRepository.cs
│   ├── IThemeRepository.cs
│   ├── ThemeRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
├── Configuration/                   # Setup and configuration
│   └── ServiceConfiguration.cs      # DI container setup
├── Exceptions/                      # Custom exception types
│   └── ComponentLibraryException.cs # Exception hierarchy
├── Constants/                       # Application constants
│   └── ApplicationConstants.cs      # Global constants
├── BlazorComponentLibrary.csproj   # Project file
├── LICENSE                          # MIT License
├── .gitignore                       # Git ignore rules
└── README.md                        # This file
```

## Features

### Components & Models
- **Data Tables** - Sortable, filterable columns with pagination
- **Charts** - Line, Bar, Pie, Doughnut, Area, Scatter, Bubble, Radar
- **Forms** - Fields with validation, custom validators, type safety
- **Modals** - Configurable dialogs with multiple size/type variants
- **Themes** - Light/Dark mode with CSS variable generation

### Services
- Full CRUD operations for all entities
- Business logic validation and rules
- Pagination and filtering
- Search capabilities
- Statistics and aggregations
- User authentication and authorization

### Data Access
- Repository pattern with dependency injection
- In-memory storage (replace with DB as needed)
- Async/await throughout
- Full error handling

## Getting Started

### Installation

Add to your Blazor project:

```csharp
using BlazorComponentLibrary.Configuration;

// In Program.cs
builder.Services.AddBlazorComponentLibrary();
```

### Usage Example

```csharp
// Inject services
[Inject]
public ComponentService ComponentService { get; set; } = null!;

// Create a component
var config = new ComponentConfig
{
    Name = "MyDataTable",
    ComponentType = "DataTable",
    Description = "User data table"
};

var created = await ComponentService.CreateComponentAsync(config);

// Retrieve all components
var components = await ComponentService.GetAllComponentsAsync();
```

## Architecture

### Layered Design
1. **Models** - Domain entities with validation and business logic
2. **Services** - Application business logic and workflows
3. **Repositories** - Data persistence abstraction
4. **Configuration** - Dependency injection setup

### Design Patterns
- **Repository Pattern** - Data access abstraction
- **Service Layer Pattern** - Business logic separation
- **Dependency Injection** - Loose coupling, testability
- **Result Pattern** - Standardized API responses
- **Validation Pattern** - Input validation and error handling

## API Examples

### Component Management
```csharp
var service = _componentService;

// Create
var config = await service.CreateComponentAsync(new ComponentConfig 
{ 
    Name = "Table", 
    ComponentType = "DataTable" 
});

// Read
var component = await service.GetComponentByIdAsync(1);

// Update
await service.UpdateComponentAsync(1, config);

// Delete
await service.DeleteComponentAsync(1);

// Search
var results = await service.SearchComponentsAsync("data");

// Statistics
var stats = await service.GetComponentStatisticsAsync();
```

### User Authentication
```csharp
// Create user
var user = await _userService.CreateUserAsync(
    "john_doe",
    "john@example.com",
    "password123"
);

// Authenticate
var authenticated = await _userService.AuthenticateAsync(
    "john_doe",
    "password123"
);

// Manage roles
await _userService.UpdateRoleAsync(userId, UserRole.Admin);
```

### Form Validation
```csharp
// Create form fields
var emailField = new FormField
{
    Name = "email",
    Label = "Email Address",
    FieldType = FormFieldType.Email,
    IsRequired = true
};

await _formService.CreateFieldAsync(emailField);

// Validate form
var result = await _formService.ValidateFormAsync(formData);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.Key}: {error.Value}");
    }
}
```

## Error Handling

The library uses a custom exception hierarchy:

```csharp
ComponentLibraryException (base)
├── InvalidComponentException
├── ComponentNotFoundException
├── FormValidationException
├── UnauthorizedException
├── ForbiddenException
├── ConflictException
└── MissingDependencyException
```

## Constants and Configuration

Global settings are defined in `ApplicationConstants` and can be customized via `LibrarySettings`:

```csharp
var settings = new LibrarySettings
{
    LibraryName = "Custom Name",
    EnableCaching = true,
    DefaultPageSize = 50,
    EnableLogging = true
};
```

## Development

Built on .NET 10.0 with C# 13 latest features.

### Building
```bash
dotnet build
```

### Testing
```bash
dotnet test
```

## Contributing

Contributions are welcome. Please maintain the existing code structure and patterns.

## Support

For issues or questions, visit https://sarmkadan.com

## License

MIT License - Copyright 2026 Vladyslav Zaiets. See LICENSE file for details.
