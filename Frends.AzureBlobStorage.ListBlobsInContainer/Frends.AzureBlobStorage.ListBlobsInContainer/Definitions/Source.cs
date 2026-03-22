using Frends.Common.Toolkit.Attributes;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// The name of the blob container in the storage account.
    /// </summary>
    /// <example>ExampleContainer</example>
    [NotEmptyString]
    public string ContainerName { get; set; }
}
