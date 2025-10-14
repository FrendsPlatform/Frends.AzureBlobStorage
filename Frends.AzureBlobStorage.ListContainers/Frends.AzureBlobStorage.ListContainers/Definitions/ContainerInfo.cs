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
    public string Name { get; set; }

    /// <summary>
    /// Public access level.
    /// </summary>
    public string PublicAccess { get; set; }

    /// <summary>
    /// Last modified timestamp.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// ETag value.
    /// </summary>
    public string ETag { get; set; }

    /// <summary>
    /// Lease status.
    /// </summary>
    public string LeaseStatus { get; set; }

    /// <summary>
    /// Lease state.
    /// </summary>
    public string LeaseState { get; set; }
}
