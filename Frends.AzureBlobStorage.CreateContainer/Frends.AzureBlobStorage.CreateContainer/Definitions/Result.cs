namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Container created.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// URI string of newly created container.
    /// </summary>
    /// <example>https://test.blob.core.windows.net/test8f237ae0-ad33-b4b3-48d9-23b20a14c909</example>
    public string Uri { get; set; } = string.Empty;


    /// <summary>
    /// Error information if operation failed.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}