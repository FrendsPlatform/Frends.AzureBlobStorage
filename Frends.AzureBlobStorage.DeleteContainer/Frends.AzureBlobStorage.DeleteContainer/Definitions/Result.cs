namespace Frends.AzureBlobStorage.DeleteContainer.Definitions
{
    public class Result
    {
        /// <summary>
        ///     Result as boolean. Returns true when action is succesful, return false when action isn't successful.
        /// </summary>
        public bool Success { get; private set; }

        public Result(bool success) {
            this.Success = success;
        }
    }
}
