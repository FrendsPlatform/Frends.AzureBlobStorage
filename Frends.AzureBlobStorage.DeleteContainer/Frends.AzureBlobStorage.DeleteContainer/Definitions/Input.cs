using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteContainer.Definitions
{
    public class Input
    {
        /// <summary>
        ///     Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        [PasswordPropertyText]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Name of the azure blob storage container which will be deleted.
        ///     Naming: lowercase
        ///     Valid chars: alphanumeric and dash, but cannot start or end with dash
        /// </summary>
        [DefaultValue("test-container")]
        [DisplayName("Container Name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }
    }
}
