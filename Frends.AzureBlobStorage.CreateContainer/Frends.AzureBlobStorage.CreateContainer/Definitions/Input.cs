using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions
{
    public class Input
    {
        /// <summary>
        ///     Connection string to Azure storage
        /// </summary>
        [DefaultValue("UseDevelopmentStorage=true")]
        [DisplayName("Connection String")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ConnectionString { get; set; }

        /// <summary>
        ///     Name of the azure blob storage container where the data will be uploaded.
        ///     Naming: lowercase
        ///     Valid chars: alphanumeric and dash, but cannot start or end with dash
        /// </summary>
        [DefaultValue("test-container")]
        [DisplayName("Container Name")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ContainerName { get; set; }
    }
}
