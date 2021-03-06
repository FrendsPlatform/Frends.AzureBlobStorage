namespace Frends.AzureBlobStorage.UploadBlob
{
    /// <summary>
    /// Result-class for UploadBlob-task.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// File which was uploaded to Azure.
        /// </summary>
        public string SourceFile { get; set; }

        /// <summary>
        /// URI of the blob.
        /// </summary>
        public string Uri { get; set; }
    }
}
