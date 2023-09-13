namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// The exception that is thrown when an error occurs in the ToastService.
/// This includes issues with toast message validation and timer management.
/// </summary>
public sealed class ToastServiceException : BlazorComponentLibraryException
{
    /// <summary>Initializes a new instance of the <see cref="ToastServiceException"/> class.</summary>
    public ToastServiceException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ToastServiceException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public ToastServiceException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ToastServiceException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ToastServiceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
