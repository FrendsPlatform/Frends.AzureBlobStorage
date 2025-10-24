namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>CreateContainer failed</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>null</example>
    public object AdditionalInfo { get; set; }
}