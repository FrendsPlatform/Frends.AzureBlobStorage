using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    /// Source values.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// Authentication method to use when connecting to Azure Storage. Options are connection string and SAS Token.
        /// </summary>
        [DefaultValue (AuthenticationMethod.ConnectionString)]
        public AuthenticationMethod AuthenticationMethod { get; set; }

        /// <summary>
        /// The base URI for the Azure storage container.
        /// </summary>
        /// <example>https://storageaccount.blob.core.windows.net</example>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.SASToken)]
        public string URI { get; set; }

        /// <summary>
        /// A shared access signature for Azure storage container. Grants restricted access rights to Azure Storage resources when combined with URI.
        /// </summary>
        /// <example>sv=2021-04-10&amp;se=2022-04-10T10%3A431Z&amp;sr=c&amp;sp=l&amp;sig=ZJg983RovE%2BZXI</example>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.SASToken)]
        [PasswordPropertyText]
        public string SASToken { get; set; }


        /// <summary>
        /// Azure storage account's connection string.
        /// </summary>
        /// <example>"DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net"</example>
        [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ConnectionString)]
        [PasswordPropertyText]
        public string ConnectionString { get; set; }


        /// <summary>
        /// Azure storage container's name.
        /// </summary>
        public string ContainerName { get; set; }
    }
}
