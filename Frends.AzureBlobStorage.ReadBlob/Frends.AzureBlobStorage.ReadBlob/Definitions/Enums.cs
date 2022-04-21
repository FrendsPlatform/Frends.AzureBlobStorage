
namespace Frends.AzureBlobStorage.ReadBlob.Definitions
{
    /// <summary>
    /// Encoding name in which blob content is read. 
    /// </summary>
    public enum Encode
    {
        UTF8,
        UTF32,
        Unicode,
        ASCII
    }

    /// <summary>
    /// Authentication options.
    /// </summary>
    public enum AuthenticationMethod
    {
        /// <summary>
        /// Authenticate with connectiong string.
        /// </summary>
        ConnectionString,

        /// <summary>
        /// Authenticate with SAS Token. Requires Storage URI.
        /// </summary>
        SASToken
    }
}
