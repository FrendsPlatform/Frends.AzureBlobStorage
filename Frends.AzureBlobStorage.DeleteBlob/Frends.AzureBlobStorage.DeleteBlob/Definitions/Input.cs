using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteBlob
{
    /// <summary>
    /// Input-class for DeleteBlob-task.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Name of the blob to delete
        /// </summary>
        [DisplayName("Blob name")]
        [DefaultValue("TestFile.xml")]
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        /// <summary>
        /// Connection string to Azure storage.
        /// </summary>
        [PasswordPropertyText]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the container where delete blob exists.
        /// </summary>
        [DisplayName("Blob Container Name")]
        [DefaultValue("test-container")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }
    }
}
