using System;

namespace Frends.AzureBlobStorage.ListContainers.Definitions;

/// <summary>
/// Represents information about an Azure Blob Storage container.
/// </summary>
public class ContainerInfo
{
    /// <summary>
    /// Container name.
    /// </summary>
    /// <example>"mycontainer01"</example>
    public string Name { get; set; }

    /// <summary>
    /// Public access level.
    /// </summary>
    /// <example>"Blob"</example>
    public string PublicAccess { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    /// <example>2025-01-15T10:23:45Z</example>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// ETag value.
    /// </summary>
    /// <example>"0x8D12345ABCDEFF"</example>
    public string ETag { get; set; }

    /// <summary>
    /// Lease status.
    /// </summary>
    /// <example>"Unlocked"</example>
    public string LeaseStatus { get; set; }

    /// <summary>
    /// Lease state.
    /// </summary>
    /// <example>"Available"</example>
    public string LeaseState { get; set; }
}