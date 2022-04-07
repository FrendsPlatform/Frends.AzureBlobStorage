using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#pragma warning disable 1591
namespace Frends.AzureBlobStorage.ReadBlob
{
    public class Source
    {
        /// <summary>
        ///     Use either Uri+SAS Token or Connection string
        /// </summary>
        [DefaultValue("https://xx.blob.xx.xx.net/")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Uri { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        public string SasToken { get; set; }

        /// <summary>
        ///     Use either Uri+SAS Token or Connection string
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Name of the azure blob storage container where the file is downloaded from.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        ///     Name of the blob to download
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string BlobName { get; set; }

        [DefaultValue("UTF8")]
        [DisplayFormat(DataFormatString = "Text")]
        public Encode Encoding { get; set; }
    }
}
