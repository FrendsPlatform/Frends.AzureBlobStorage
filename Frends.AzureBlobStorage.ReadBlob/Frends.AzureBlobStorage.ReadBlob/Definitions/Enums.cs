using System.ComponentModel;
#pragma warning disable 1591
namespace Frends.AzureBlobStorage.ReadBlob
{
    [DefaultValue("UTF-8")]
    public enum Encode
    {
        UTF8,
        UTF32,
        Unicode,
        ASCII
    }
}
