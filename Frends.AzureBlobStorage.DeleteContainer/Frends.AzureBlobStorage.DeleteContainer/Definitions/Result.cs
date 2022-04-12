namespace Frends.AzureBlobStorage.DeleteContainer.Definitions
{
    public class Result
    {
        /// <summary>
        ///     Result as boolean. Returns true when action was succesful, return false when action wasn't successful.
        /// </summary>
        public bool Success { get; private set; }

        public Result(bool success) {
            this.Success = success;
        }
    }
}
