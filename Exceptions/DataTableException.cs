namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// The exception that is thrown when a <c>DataTable</c> receives data it refuses to render,
/// such as a source that exceeds the configured maximum row count.
/// </summary>
public sealed class DataTableException : BlazorComponentLibraryException
{
    /// <summary>Initializes a new instance of the <see cref="DataTableException"/> class.</summary>
    public DataTableException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DataTableException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public DataTableException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DataTableException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public DataTableException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
