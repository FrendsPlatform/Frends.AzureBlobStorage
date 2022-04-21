
namespace Frends.AzureBlobStorage.ReadBlob.Definitions
{
    /// <summary>
    /// Encoding name in which blob content is read. 
    /// </summary>
    public enum Encode
    {
    #pragma warning disable CS1591 // Encoding options. No need for XML here.
        UTF8,
        UTF32,
        Unicode,
        ASCII
    #pragma warning restore CS1591
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
