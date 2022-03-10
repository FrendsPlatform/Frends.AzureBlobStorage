using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Frends.AzureBlobStorage.DeleteBlob
{
    /// <summary>
    /// Task class.
    /// </summary>
    public class AzureBlobStorage
    {
        /// <summary>
        /// Deletes a single blob from Azure blob storage.
        /// [Documentation](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/tree/main/Frends.AzureBlobStorage.DeleteBlob)
        /// </summary>
        /// <param name="target">Blob which will be deleted.</param>
        /// <param name="connectionProperties">Connection options.</param>
        /// <param name="cancellationToken"/>
        /// <returns>Object { bool Success }</returns>
        public static async Task<Result> DeleteBlob(
            [PropertyTab] Input target,
            [PropertyTab] Options connectionProperties,
            CancellationToken cancellationToken)
        {

            // Get Blob Client.
            var blob = new BlobClient(target.ConnectionString, target.ContainerName, target.BlobName);

            if (!await blob.ExistsAsync(cancellationToken)) return new Result {Success = true};

            try
            {
                var accessCondition = string.IsNullOrWhiteSpace(connectionProperties.VerifyETagWhenDeleting)
                    ? new BlobRequestConditions { IfMatch = new Azure.ETag(connectionProperties.VerifyETagWhenDeleting) }
                    : null;

                var result = await blob.DeleteIfExistsAsync(
                    connectionProperties.SnapshotDeleteOption.ConvertEnum<DeleteSnapshotsOption>(), accessCondition,
                    cancellationToken);

                return new Result {Success = result};
            }
            catch (Exception e)
            {
                throw new Exception("DeleteBlobAsync: Error occured while trying to delete blob", e);
            }
        }
    }
}
