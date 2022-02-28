using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DownloadBlob
{
    /// <summary>
    /// Input-class for DownloadBlob-task.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Connection string to Azure storage.
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the azure blob storage container where the file is downloaded from.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        /// Name of the blob to download.
        /// </summary>
        [DefaultValue("example.xml")]
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        /// <summary>
        /// Set encoding manually.
        /// Empty value tries to get encoding set in Azure.
        /// Supported values are utf-8, utf-7, utf-32, unicode, bigendianunicode and ascii.
        /// </summary>
        [DefaultValue("UTF-8")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Encoding { get; set; }
    }
}
