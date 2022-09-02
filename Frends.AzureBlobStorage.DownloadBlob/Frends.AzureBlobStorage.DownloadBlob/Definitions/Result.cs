namespace Frends.AzureBlobStorage.DownloadBlob.Definitions;

/// <summary>
/// Result-class for DownloadBlob-task.
/// </summary>
public class Result
{
    /// <summary>
    /// Name of the downloaded file.
    /// </summary>
    /// <example>sample.txt</example>
    public string FileName { get; private set; }

    /// <summary>
    /// Directory where the file was downloaded.
    /// </summary>
    /// <example>c:\temp</example>
    public string Directory { get; private set; }

    /// <summary>
    /// Full path to the downloaded file.
    /// </summary>
    /// <example>c:\temp\sample.txt</example>
    public string FullPath { get; private set; }

    internal Result(string fileName, string directory, string fullPath)
    {
        FileName = fileName;
        Directory = directory;
        FullPath = fullPath;
    }
}