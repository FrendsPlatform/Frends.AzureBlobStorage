#pragma warning disable 1591
namespace Frends.AzureBlobStorage.ReadBlob
{
    public class Result
    {
        public string Content { get; set; }
        public Result(string content)
        {
            Content = content;
        }
    }
}
