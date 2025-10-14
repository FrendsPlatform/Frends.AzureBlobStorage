namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Error that occurred during the task.
/// </summary>
public class Error
{
    /// <summary>
    /// Summary of the error.
    /// </summary>
    /// <example>Unable to join strings.</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional information about the error.
    /// </summary>
    /// <example>object { Exception Exception }</example>
    // TODO: Add task specific additional information. Strong typing is recommended when reasonable.
    public dynamic AdditionalInfo { get; set; }
}
