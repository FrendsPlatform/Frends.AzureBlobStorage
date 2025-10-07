namespace Frends.AzureBlobStorage.WriteBlob.Definitions;

/// <summary>
/// Task's result.
/// </summary>
public class Result
{
    /// <summary>
    /// Operation complete.
    /// Operation is seens as completed if an ignorable error has occured and Options.ThrowErrorOnFailure is set to false.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; private set; }

    /// <summary>
    /// This object contains the source file path and the URL of the blob. 
    /// If an ignorable error occurs, such as when a blob already exists and Options.ThrowErrorOnFailure is set to false, the URL will be replaced with the corresponding error message.age.
    /// </summary>
    /// <example>{ { c:\temp\examplefile.txt, https://storage.blob.core.windows.net/container/examplefile.txt }, { c:\temp\examplefile2.txt, Blob examplefile2 already exists. } }</example>
    public string Info { get; private set; }

    /// <summary>
    /// URI of uploaded file.
    /// </summary>
    public string Uri { get; private set; }

    internal Result(bool success, string info, string uri)
    {
        Success = success;
        Info = info;
        Uri = uri;
    }
}