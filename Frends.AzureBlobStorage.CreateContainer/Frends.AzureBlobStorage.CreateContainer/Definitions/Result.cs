namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    /// <summary>
    /// Result-class for DownloadBlob-task.
    /// </summary>
    public class Result
    {
        /// <summary>
        ///     Uri string of newly created container.
        /// </summary>
        public string Uri { get; private set; }

        public Result(string uri) {
            this.Uri = uri;
        }
    }
}
