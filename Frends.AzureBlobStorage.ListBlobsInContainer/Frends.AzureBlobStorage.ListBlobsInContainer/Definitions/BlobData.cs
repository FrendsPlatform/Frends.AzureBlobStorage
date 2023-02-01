namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

/// <summary>
/// Blob's data.
/// </summary>
public class BlobData
{
    /// <summary>
    /// Blob's type.
    /// </summary>
    /// <example>Block</example>
    public string Type { get; set; }

    /// <summary>
    /// Blob's name.
    /// </summary>
    /// <example>file.txt, directory/file.txt</example>
    public string Name { get; set; }

    /// <summary>
    /// Blob's URL.
    /// </summary>
    /// <example>https://storageaccount.blob.core.windows.net/containername/file.txt</example>
    public string URL { get; set; }

    /// <summary>
    /// Blob's entity tag.
    /// </summary>
    /// <example>0x8D6FACDEB709651</example>
    public string ETag { get; set; }
}