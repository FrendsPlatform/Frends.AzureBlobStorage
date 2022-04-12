namespace Frends.AzureBlobStorage.DeleteContainer.Definitions
{
    public class Result
    {
        /// <summary>
        ///     Result as boolean. Returns true when contain has been deleted, return false when action isn't successful or container is not found.
        /// </summary>
        public bool ContainerWasDeleted { get; private set; }

        /// <summary>
        ///     Message string of description about action's result.
        /// </summary>
        public string Message { get; private set; }

        public Result(bool containerWasDeleted, string message) {
            this.ContainerWasDeleted = containerWasDeleted;
            this.Message = message;
        }
    }
}
