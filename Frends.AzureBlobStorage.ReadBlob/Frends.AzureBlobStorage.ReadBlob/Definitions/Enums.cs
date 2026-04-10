namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Encoding name in which blob content is read.
/// </summary>
public enum Encode
{
#pragma warning disable CS1591 // self explanatory
    UTF8,
    UTF32,
    Unicode,
    ASCII
#pragma warning restore CS1591 // self explanatory
}
