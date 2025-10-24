using System.Collections.Generic;

namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Result of the task.
/// </summary>
public class Result
{
    /// <summary>
    /// Indicates if the task completed successfully.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// List of containers returned by the operation.
    /// </summary>
    /// <example>
    /// [
    ///   {
    ///     "Name": "test-container",
    ///     "PublicAccess": "Private",
    ///     "LastModified": "2025-01-15T10:23:45Z",
    ///     "ETag": "0x8D12345ABCDEFF",
    ///     "LeaseStatus": "Unlocked",
    ///     "LeaseState": "Available"
    ///   }
    /// ]
    /// </example>
    public List<ContainerInfo> Containers { get; set; }

    /// <summary>
    /// Error that occurred during task execution.
    /// </summary>
    /// <example>object { string Message, Exception AdditionalInfo }</example>
    public Error Error { get; set; }
}
