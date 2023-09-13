namespace BlazorComponentLibrary.Exceptions;

/// <summary>
/// The exception that is thrown when there is a configuration error in the BlazorComponentLibrary.
/// This includes invalid configuration values, missing required settings, or invalid combinations of settings.
/// </summary>
public sealed class ConfigurationException : BlazorComponentLibraryException
{
    /// <summary>Initializes a new instance of the <see cref="ConfigurationException"/> class.</summary>
    public ConfigurationException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public ConfigurationException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ConfigurationException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public ConfigurationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
