namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// Base exception for all exceptions thrown by BlazorComponentLibrary.
/// </summary>
public abstract class BlazorComponentLibraryException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="BlazorComponentLibraryException"/> class.</summary>
    protected BlazorComponentLibraryException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="BlazorComponentLibraryException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    protected BlazorComponentLibraryException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="BlazorComponentLibraryException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    protected BlazorComponentLibraryException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
