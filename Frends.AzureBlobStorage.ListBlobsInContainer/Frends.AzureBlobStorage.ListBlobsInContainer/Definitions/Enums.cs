namespace Frends.AzureBlobStorage.ListBlobsInContainer.Definitions
{
    /// <summary>
    ///     Authentication options
    /// </summary>
    public enum AuthenticationMethod
    {
        /// <summary>
        ///     Authenticate with connectiong string.
        /// </summary>
        Connectionstring,

        /// <summary>
        /// Authenticate with SAS Token. Requires Storage URI.
        /// </summary>
        Sastoken
    }
}
