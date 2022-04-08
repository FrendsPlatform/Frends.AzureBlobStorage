namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    /// <summary>
    /// Result-class for DownloadBlob-task.
    /// </summary>
    public class Result
    {
        public string SourceFile { get; private set; }
        public string Uri { get; private set; }

        public Result(string source, string uri) {
            this.SourceFile = source;
            this.Uri = uri;
        }
    }
}
