using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteBlob
{
    /// <summary>
    /// Options-class for DeleteBlob-task.
    /// </summary>
    public class Options
    {
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
