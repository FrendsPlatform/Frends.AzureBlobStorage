using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Input parameters
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the Azure Blob Storage container where the data will be uploaded.
    /// Naming: lowercase
    /// Valid chars: alphanumeric and dash, but cannot start or end with dash.
    /// </summary>
    /// <example>test-container</example>
    [DefaultValue("test-container")]
    [DisplayFormat(DataFormatString = "Text")]
    public string ContainerName { get; set; }
}
