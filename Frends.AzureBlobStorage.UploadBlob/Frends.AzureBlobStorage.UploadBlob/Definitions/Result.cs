namespace Frends.AzureBlobStorage.UploadBlob;

/// <summary>
/// Result-class for UploadBlob-task.
/// </summary>
public class Result
{
    /// <summary>
    /// File which was uploaded to Azure.
    /// </summary>
    /// <example>c:\temp\testfile.txt</example>
    public string SourceFile { get; set; }

    /// <summary>
    /// URI of the blob.
    /// </summary>
    /// <example>https://storage.blob.core.windows.net/container/testfile.txt</example>
    public string Uri { get; set; }
}