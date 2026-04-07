using System.ComponentModel.DataAnnotations;

namespace Frends.AzureBlobStorage.UploadBlob.Attributes;

/// <summary>
/// Validates that a string property is not null or empty.
/// This attribute is a specialization of the RequiredAttribute with AllowEmptyStrings set to false.
/// </summary>
public class NotEmptyStringAttribute : RequiredAttribute
{
    /// <inheritdoc/>
    public NotEmptyStringAttribute()
    {
        AllowEmptyStrings = false;
        ErrorMessage = "{0} is required and cannot be empty.";
    }
}
