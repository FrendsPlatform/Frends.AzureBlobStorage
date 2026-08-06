using Frends.AzureBlobStorage.ListBlobsInContainer.Attributes;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// The name of the blob container in the storage account.
    /// </summary>
    /// <example>ExampleContainer</example>
    [NotEmptyString]
    public string ContainerName { get; set; }
}
