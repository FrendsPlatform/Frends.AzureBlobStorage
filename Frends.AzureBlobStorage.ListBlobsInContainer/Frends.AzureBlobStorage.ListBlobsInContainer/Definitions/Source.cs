using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    ///     Source values.
    /// </summary>
    public class Source
    {
        /// <summary>
        ///     Authentication method to use when connecting to Azure Storage. Options are connection string and SAS Token.
        /// </summary>
        public AuthenticationMethod AuthenticationMethod { get; set; }

        /// <summary>
        ///     The base URI for the storage container.
        /// </summary>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.Sastoken)]
        [DefaultValue("https://storageaccount.blob.core.windows.net/")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Uri { get; set; }

        /// <summary>
        ///     A shared access signature for storage container. Grants restricted access rights to Azure Storage resources when combined with URI.
        /// </summary>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.Sastoken)]
        [PasswordPropertyText]
        [DisplayFormat(DataFormatString = "Text")]
        public string SasToken { get; set; }


        /// <summary>
        ///     Storage container's connection string.
        /// </summary>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.Connectionstring)]
        [PasswordPropertyText]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }


        /// <summary>
        ///     Storage container's name.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }

        /// <summary>
        ///     List blobs in a flat listing structure, or hierarchically. A hierarchical listing returns blobs as though they were organized into folders.
        /// </summary>
        public bool FlatBlobListing { get; set; }

        /// <summary>
        ///     Specify a prefix to return blobs whose names begin with that character or string.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Prefix { get; set; }

    }
}
