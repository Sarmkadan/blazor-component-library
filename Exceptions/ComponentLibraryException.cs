// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// Base exception for all component library errors.
/// Inheritors should use this as a base for specific library exceptions.
/// </summary>
public class ComponentLibraryException : Exception
{
    public string? ErrorCode { get; set; }
    public int? ErrorStatusCode { get; set; }

    public ComponentLibraryException(string message) : base(message)
    {
    }

    public ComponentLibraryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ComponentLibraryException(string message, string errorCode, int? statusCode = null)
        : base(message)
    {
        ErrorCode = errorCode;
        ErrorStatusCode = statusCode;
    }
}

/// <summary>
/// Thrown when a component configuration is invalid.
/// </summary>
public class InvalidComponentException : ComponentLibraryException
{
    public InvalidComponentException(string message)
        : base(message, "INVALID_COMPONENT", 400)
    {
    }
}

/// <summary>
/// Thrown when a requested component is not found.
/// </summary>
public class ComponentNotFoundException : ComponentLibraryException
{
    public int ComponentId { get; set; }

    public ComponentNotFoundException(int id)
        : base($"Component with ID {id} not found", "COMPONENT_NOT_FOUND", 404)
    {
        ComponentId = id;
    }
}

/// <summary>
/// Thrown when form validation fails.
/// </summary>
public class FormValidationException : ComponentLibraryException
{
    public Dictionary<string, string> FieldErrors { get; set; } = new();

    public FormValidationException(string message, Dictionary<string, string>? errors = null)
        : base(message, "VALIDATION_ERROR", 400)
    {
        if (errors != null)
        {
            FieldErrors = errors;
        }
    }
}

/// <summary>
/// Thrown when a user is unauthorized or authentication fails.
/// </summary>
public class UnauthorizedException : ComponentLibraryException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

/// <summary>
/// Thrown when access is forbidden.
/// </summary>
public class ForbiddenException : ComponentLibraryException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, "FORBIDDEN", 403)
    {
    }
}

/// <summary>
/// Thrown when a resource conflicts with existing data.
/// </summary>
public class ConflictException : ComponentLibraryException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", 409)
    {
    }
}

/// <summary>
/// Thrown when a required dependency is missing.
/// </summary>
public class MissingDependencyException : ComponentLibraryException
{
    public string? DependencyName { get; set; }

    public MissingDependencyException(string message, string? dependencyName = null)
        : base(message, "MISSING_DEPENDENCY", 500)
    {
        DependencyName = dependencyName;
    }
}

/// <summary>
/// Represents an API error response.
/// </summary>
public class ApiErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public int StatusCode { get; set; }
    public Dictionary<string, string>? Details { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiErrorResponse FromException(ComponentLibraryException ex)
    {
        return new ApiErrorResponse
        {
            Message = ex.Message,
            ErrorCode = ex.ErrorCode,
            StatusCode = ex.ErrorStatusCode ?? 500,
            Details = ex is FormValidationException fve ? fve.FieldErrors : null,
            Timestamp = DateTime.UtcNow
        };
    }
}
