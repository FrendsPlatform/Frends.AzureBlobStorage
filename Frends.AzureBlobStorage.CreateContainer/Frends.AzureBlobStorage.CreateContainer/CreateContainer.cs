using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Frends.AzureBlobStorage.CreateContainer.Definitions;
using static Frends.AzureBlobStorage.CreateContainer.Definitions.Enums;

namespace Frends.AzureBlobStorage.CreateContainer
{
    public static class AzureBlobStorage
    {
        /// <summary>
        /// Downloads Blob to a file.
        /// [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.CreateContainer)
        /// </summary>
        /// <param name="source">Information about which Blob to download.</param>
        /// <param name="destination">Information about the download destination.</param>
        /// <returns>Object { string FileName, string Directory, string FullPath}</returns>
        /// <summary>
        ///     Uploads a single file to Azure blob storage. See https://github.com/CommunityHiQ/Frends.Community.Azure.Blob
        ///     Will create given container on connection if necessary.
        /// </summary>
        /// <returns>Object { string Uri, string SourceFile }</returns>
        public static async Task<Result> CreateContainer([PropertyTab] Input input,
            [PropertyTab] Destination destinationProperties, CancellationToken cancellationToken)
        {
            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();

            // check that source file exists
            var fi = new FileInfo(input.SourceFile);
            if (!fi.Exists)
                throw new ArgumentException($"Source file {input.SourceFile} does not exist", nameof(input.SourceFile));

            // get container
            var container = Utils.GetBlobContainer(destinationProperties.ConnectionString,
                destinationProperties.ContainerName);

            // check for interruptions
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (destinationProperties.CreateContainerIfItDoesNotExist)
                    await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Checking if container exists or creating new container caused an exception.", ex);
            }

            string fileName;
            if (string.IsNullOrWhiteSpace(destinationProperties.RenameTo) && input.Compress)
            {
                fileName = fi.Name + ".gz";
            }
            else if (string.IsNullOrWhiteSpace(destinationProperties.RenameTo))
            {
                fileName = fi.Name;
            }
            else
            {
                fileName = destinationProperties.RenameTo;
            }

            // return uri to uploaded blob and source file path

            switch (destinationProperties.BlobType)
            {
                case AzureBlobType.Append:
                    return await AppendBlob(input, destinationProperties, fi, fileName, cancellationToken);
                case AzureBlobType.Page:
                    return await UploadPageBlob(input, destinationProperties, fi, fileName, cancellationToken);
                default:
                    return await UploadBlockBlob(input, destinationProperties, fi, fileName, cancellationToken);
            }
        }

    }
}
