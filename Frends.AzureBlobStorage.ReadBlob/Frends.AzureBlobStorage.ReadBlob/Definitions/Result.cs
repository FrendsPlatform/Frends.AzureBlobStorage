namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Task's result.
/// object { bool Success, string Content, Error Error }
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates whether the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// Encoded blob content.
    /// </summary>
    /// <example>"line1\r\nline2\r\nline3"</example>
    public string Content { get; set; }

    /// <summary>
    /// Error details when Success is false.
    /// object { string Message, object AdditionalInfo }
    /// </summary>
    /// <example>null</example>
    public Error Error { get; set; }
}
