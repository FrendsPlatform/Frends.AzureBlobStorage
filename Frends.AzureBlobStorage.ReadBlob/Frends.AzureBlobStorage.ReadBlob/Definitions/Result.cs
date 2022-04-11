#pragma warning disable 1591

namespace Frends.AzureBlobStorage.ReadBlob.Definitions
{
    /// <summary>
    ///     Returns encoded blob content.
    /// </summary>
    public class Result
    {
        /// <summary>
        ///     Encoded blob content.
        /// </summary>
        public string Content { get; private set; }

        public Result(string content)
        {
            Content = content;
        }
    }
}

