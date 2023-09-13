namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// The exception that is thrown when validation of input data fails.
/// This includes invalid parameter values, failed validation rules, or invalid data formats.
/// </summary>
public sealed class ValidationException : BlazorComponentLibraryException
{
    /// <summary>Initializes a new instance of the <see cref="ValidationException"/> class.</summary>
    public ValidationException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public ValidationException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ValidationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}