using System.ComponentModel;
namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Option parameters.
/// </summary>
public class Options
{
    /// <summary>
    /// Throw an error if container to be deleted doesn't exists.
    /// </summary>
    [DefaultValue(false)]
    public bool ThrowErrorIfContainerDoesNotExists { get; set; }
}
