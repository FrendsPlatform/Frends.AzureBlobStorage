namespace Frends.AzureBlobStorage.UploadBlob.Definitions;

/// <summary>
/// Result.
/// </summary>
public class Result
{
    /// <summary>
    /// Uploaded file.
    /// </summary>
    /// <example>c:\temp\testfile.txt</example>
    public string SourceFile { get; private set; }

    /// <summary>
    /// Blob's URI.
    /// </summary>
    /// <example>https://storage.blob.core.windows.net/container/testfile.txt</example>
    public string Uri { get; private set; }

    internal Result(string sourceFile, string uri)
    {
        SourceFile = sourceFile;
        Uri = uri;
    }
}