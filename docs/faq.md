# Frequently Asked Questions

## General Questions

### Q: What is the Blazor Component Library?

A: It's a lightweight, open-source .NET library providing reusable Blazor components (data tables, charts, forms, modals) without heavy CSS framework dependencies. Built on .NET 10 with C# 13, it emphasizes type safety, extensibility, and production-readiness.

### Q: Can I use this with Blazor Server and Blazor WebAssembly?

A: Yes! The library works with both Blazor Server and WebAssembly hosting models. The core services are framework-agnostic and can be shared between hosting scenarios.

### Q: Is this a UI component library or a service library?

A: Both. It provides:
- **Services** - Business logic, data management, validation (core library)
- **Models** - Strongly-typed data structures
- **Integration** - Easy to integrate with your own UI components or component libraries

The library handles the logic; you control the UI rendering.

### Q: What's included in the free version?

A: Everything is included - it's fully open-source under the MIT license. No paid tiers or feature restrictions.

### Q: Does this work with other CSS frameworks like Bootstrap or Tailwind?

A: Yes! The library has zero CSS dependencies. You can style components using:
- Bootstrap classes
- Tailwind CSS
- Material Design
- Custom CSS
- Any framework you prefer

### Q: How do I migrate from another component library?

A: The library follows standard .NET patterns (repositories, services, DI). Migration involves:
1. Swapping service registrations
2. Updating component templates
3. Adjusting CSS/styling

See [Getting Started](getting-started.md) for examples.

---

## Installation & Setup

### Q: How do I install the library?

A: Once published to NuGet:
```bash
dotnet add package BlazorComponentLibrary
```

For local development:
```bash
git clone https://github.com/sarmkadan/blazor-component-library.git
# Add project reference in your .csproj
```

### Q: What are the minimum .NET version requirements?

A: .NET 10 or later. Earlier versions are not supported.

### Q: Do I need to run any migrations or setup?

A: No! The library works out-of-the-box with in-memory storage. Just call `AddBlazorComponentLibrary()` in `Program.cs`.

### Q: Can I use this in an ASP.NET Core MVC app?

A: The Blazor components are specific to Blazor apps. However, the services layer can be used in any .NET application. You can:
- Use services in MVC controllers
- Call APIs from JavaScript
- Create your own UI wrapper

---

## Services & Data Access

### Q: How do I use a database instead of in-memory storage?

A: Implement custom repositories:

```csharp
public class EfComponentRepository : IComponentRepository
{
    private readonly DbContext _context;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
    {
        _context.Components.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    // Implement other methods
}

// Register in Program.cs
services.AddScoped<IComponentRepository, EfComponentRepository>();
```

### Q: Can I use the library with MongoDB?

A: Yes! Implement a custom repository:

```csharp
public class MongoComponentRepository : IComponentRepository
{
    private readonly IMongoCollection<ComponentConfig> _collection;

    public async Task<ComponentConfig> CreateAsync(ComponentConfig config)
    {
        await _collection.InsertOneAsync(config);
        return config;
    }

    // Implement other methods
}
```

### Q: How does caching work?

A: The library provides an `ICacheService` abstraction. Default is in-memory; configure via:

```csharp
services.AddBlazorComponentLibrary(options =>
{
    options.EnableCaching = true;
    options.CacheDurationMinutes = 30;
});
```

### Q: Can I disable caching?

A: Yes, set in configuration:

```csharp
services.AddBlazorComponentLibrary(options =>
{
    options.EnableCaching = false;
});
```

### Q: How do I implement my own cache provider (Redis, etc.)?

A: Implement `ICacheService`:

```csharp
public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonConvert.SerializeObject(value);
        await _redis.GetDatabase().StringSetAsync(key, json, expiry);
    }

    // Implement other methods
}

services.AddScoped<ICacheService, RedisCacheService>();
```

---

## Components & Forms

### Q: Can I customize component appearance?

A: Yes! The library provides models/services; you control the UI. Create custom Blazor components:

```csharp
@page "/custom-table"
@inject DataService DataService

<div class="my-custom-table">
    @foreach (var row in rows)
    {
        <!-- Your custom HTML/CSS -->
    }
</div>
```

### Q: How do I validate forms?

A: Use `FormService`:

```csharp
var field = new FormField
{
    Name = "email",
    FieldType = FormFieldType.Email,
    IsRequired = true
};

var result = await formService.ValidateFormAsync(data);
if (!result.IsValid)
{
    // Show errors
}
```

### Q: Can I add custom validators?

A: Yes, implement custom validation logic:

```csharp
public class CustomValidator
{
    public ValidationResult Validate(FormField field, object value)
    {
        if (field.Name == "username" && ((string)value).Length < 3)
            return ValidationResult.Failure("Username too short");

        return ValidationResult.Success();
    }
}
```

### Q: How do I handle file uploads?

A: The library doesn't include file handling. Use Blazor's `InputFile`:

```csharp
<InputFile OnChange="@OnFileSelected" />

@code {
    private async Task OnFileSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        using var stream = file.OpenReadStream();
        // Process file
    }
}
```

### Q: Can I create master-detail views?

A: Yes! Use service queries:

```csharp
@code {
    private List<Order> orders;
    private OrderDetails? selectedDetails;

    private async Task SelectOrder(int orderId)
    {
        selectedDetails = await dataService.GetOrderDetailsAsync(orderId);
    }
}
```

---

## Authentication & Authorization

### Q: How do I implement authentication?

A: Use `UserService`:

```csharp
var user = await userService.AuthenticateAsync(username, password);
if (user != null)
{
    // Set authentication state
}
```

### Q: How do I check user permissions?

A: Use `HasPermissionAsync`:

```csharp
if (await userService.HasPermissionAsync(userId, "CanDeleteUsers"))
{
    // Show delete button
}
```

### Q: Can I integrate with Active Directory/LDAP?

A: The library uses basic username/password. To use AD:

```csharp
public class ActiveDirectoryUserService : IUserService
{
    private readonly DirectoryEntry _directory;

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        // Validate against AD
        var adUser = ValidateInAd(username, password);
        if (adUser != null)
        {
            // Create or update local user
            return await GetOrCreateUserAsync(adUser);
        }
        return null;
    }

    // Implement other methods
}
```

### Q: How do I implement OAuth/OIDC?

A: Use ASP.NET Core's built-in OAuth support with the library:

```csharp
services.AddAuthentication(options =>
{
    options.DefaultScheme = "cookie";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("cookie")
.AddOpenIdConnect("oidc", options =>
{
    // OIDC configuration
});

// Library services available with authenticated user
services.AddBlazorComponentLibrary();
```

---

## Performance & Scaling

### Q: How do I improve performance with large datasets?

A: Use pagination:

```csharp
var page = await dataService.GetPagedDataAsync("users", 1, 50);
```

Enable caching:

```csharp
options.EnableCaching = true;
```

Use filtering:

```csharp
var filtered = await dataService.GetFilteredDataAsync("users", filters);
```

### Q: What's the maximum dataset size?

A: Depends on your hosting environment. The library scales based on:
- Available memory (in-memory mode)
- Database capacity (custom repository mode)
- Pagination (recommended for >10K rows)

### Q: How do I monitor performance?

A: Enable logging:

```json
{
  "Logging": {
    "LogLevel": {
      "BlazorComponentLibrary": "Debug"
    }
  }
}
```

### Q: Can I use async/await with all operations?

A: Yes! All service methods are async. Always use `await`:

```csharp
// ✅ Correct
var data = await dataService.GetTableDataAsync("users");

// ❌ Wrong - blocks thread
var data = dataService.GetTableDataAsync("users").Result;
```

### Q: What about connection pooling?

A: When using custom repositories with databases:

```csharp
services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(connectionString)
    // Connection pooling is enabled by default
);
```

---

## Troubleshooting

### Q: "NullReferenceException" when accessing injected service

A: Ensure the service is injected properly:

```csharp
// ✅ Correct
@inject ComponentService ComponentService

// ❌ Wrong - service is null outside @code
@code {
    // Can use ComponentService here
}
```

### Q: "Service not found" error at runtime

A: Verify `AddBlazorComponentLibrary()` is called in `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBlazorComponentLibrary();  // Must be before Build()
var app = builder.Build();
```

### Q: Form validation not triggering

A: Ensure `DataAnnotationsValidator` is included:

```csharp
<EditForm Model="@model" OnValidSubmit="@HandleSubmit">
    <DataAnnotationsValidator />  <!-- Required! -->
    <ValidationSummary />
    <!-- Form fields -->
</EditForm>
```

### Q: Middleware not executing

A: Ensure middleware is registered in correct order:

```csharp
app.UseRouting();
app.UseMiddleware<CustomMiddleware>();  // Before MapRazorComponents
app.MapRazorComponents<App>();
```

### Q: Changes not reflecting in UI

A: Call `StateHasChanged()` if service calls don't trigger re-render:

```csharp
@code {
    private async Task RefreshData()
    {
        data = await service.GetDataAsync();
        StateHasChanged();  // Force re-render
    }
}
```

### Q: Out of memory with caching enabled

A: Implement cache eviction:

```csharp
// Clear old entries
await cacheService.RemoveAsync("old_key");

// Or clear all
await cacheService.ClearAsync();
```

### Q: CORS errors when calling APIs

A: Enable CORS in `Program.cs`:

```csharp
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

app.UseCors();
```

---

## Best Practices

### Q: What's the recommended project structure?

A: Follow the library's pattern:

```
MyProject/
├── Pages/                    # Blazor components
├── Services/                 # Business logic wrappers
├── Models/                   # Domain models
├── Data/                     # Custom repositories (if needed)
└── Program.cs
```

### Q: Should I create one service or multiple?

A: One focused service per aggregate root:

```csharp
// ✅ Good - focused responsibility
public class OrderService { }
public class UserService { }

// ❌ Bad - too many responsibilities
public class AllService { }
```

### Q: How often should I call services?

A: Only when needed:

```csharp
// ✅ Efficient
protected override async Task OnInitializedAsync()
{
    data = await service.GetDataAsync();
}

// ❌ Inefficient - calls on every parameter change
protected override async Task OnParametersSetAsync()
{
    data = await service.GetDataAsync();
}
```

### Q: Should I catch all exceptions?

A: No, catch specific expected exceptions:

```csharp
// ✅ Good
try
{
    await service.DeleteAsync(id);
}
catch (ComponentNotFoundException)
{
    // Handle not found
}

// ❌ Bad - too broad
try
{
    await service.DeleteAsync(id);
}
catch (Exception)
{
    // Hide all errors
}
```

---

## Contributing & Support

### Q: How do I report a bug?

A: Open an issue on GitHub with:
- Minimal reproduction case
- Expected vs actual behavior
- .NET version and environment

### Q: Can I contribute code?

A: Yes! See [Contributing Guidelines](../README.md#contributing).

### Q: How do I request a feature?

A: Open a GitHub discussion or issue describing:
- Use case
- Proposed solution
- Why it's important

### Q: How often is the library updated?

A: Regular updates with:
- Bug fixes (as needed)
- Feature additions (quarterly)
- .NET version updates (within 3 months of release)

### Q: Is there commercial support?

A: Contact the maintainer at https://sarmkadan.com for support options.

---

Still have questions? Check the [Getting Started Guide](getting-started.md) or open an issue on GitHub!
