namespace Frends.AzureBlobStorage.DownloadBlob
{
    /// <summary>
    /// Result-class for DownloadBlob-task.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Name of the downloaded file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Directory where the file was downloaded.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// Full path to the downloaded file.
        /// </summary>
        public string FullPath { get; set; }
    }
}
