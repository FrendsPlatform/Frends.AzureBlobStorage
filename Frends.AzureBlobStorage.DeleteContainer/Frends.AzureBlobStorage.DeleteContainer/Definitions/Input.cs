using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.DeleteContainer.Definitions;

/// <summary>
/// Input parameters.
/// </summary>
public class Input
{
    /// <summary>
    /// Name of the Azure blob storage container which will be deleted.
    /// </summary>
    /// <example>test-container</example>
    public string ContainerName { get; set; }
}
