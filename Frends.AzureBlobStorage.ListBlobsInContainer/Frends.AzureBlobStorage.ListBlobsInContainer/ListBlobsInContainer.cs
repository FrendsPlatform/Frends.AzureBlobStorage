using System;
using System.ComponentModel;
using Azure.Storage.Blobs;
using Azure;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
#pragma warning disable CS1573

namespace Frends.AzureBlobStorage.ListBlobsInContainer
{
    public class AzureBlobStorage
    {
        /// <summary>
        ///     List of blobs in container.
        ///     [Documentation](https://tasks.frends.com/tasks#frends-tasks/Frends.AzureBlobStorage.ListBlobsInContainer)
        /// </summary>
        /// <param name="source">Source values.</param>
        /// <returns>List of blobs in container with chosen listing structure.</returns>
        /// <exception cref="Exception">Authentication exception.</exception>

        public static async Task<Result> ListBlobsInContainer([PropertyTab] Source source)
        {
            BlobContainerClient blobContainerClient = null;
            var blobsList = new List<Result>();
            var authSas = false;

            switch (source.AuthenticationMethod)
            {
                case AuthenticationMethod.Connectionstring:
                    if (string.IsNullOrEmpty(source.ConnectionString))
                        throw new Exception("Connection string required.");
                    blobContainerClient = new BlobContainerClient(source.ConnectionString, source.ContainerName);
                    break;

                case AuthenticationMethod.Sastoken:
                    if (string.IsNullOrEmpty(source.SasToken) || string.IsNullOrEmpty(source.Uri))
                        throw new Exception("SAS Token and URI required.");
                    authSas = true;
                    blobContainerClient = new BlobContainerClient(new Uri($"{source.Uri}/{source.ContainerName}?"), new AzureSasCredential(source.SasToken));
                    break;
            }

            if (source.FlatBlobListing)
            {
                var enumerable = blobContainerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, string.IsNullOrWhiteSpace(source.Prefix) ? null : source.Prefix).AsPages();
                var enumerator = enumerable.GetAsyncEnumerator();
                var flatBlobListing = await FlatBlobListing(enumerator, source, blobsList, authSas);
                return new Result { BlobList = flatBlobListing };

            }
            else
            {
                var enumerable = blobContainerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", string.IsNullOrWhiteSpace(source.Prefix) ? null : source.Prefix).AsPages();
                var enumerator = enumerable.GetAsyncEnumerator();
                var listBlobsHierarchy = await ListBlobsHierarchy(enumerator, source, blobsList, authSas);
                return new Result { BlobList = listBlobsHierarchy };
            }
        }


        /// <summary>
        ///     List blobs in a flat listing structure.
        /// </summary>
        /// <param name="source">Source values.</param>
        /// <param name="blobsList">List of blobs in container.</param>
        /// <param name="authSas">Authentication method.</param>
        /// <returns>List of blobs in container with flat listing structure.</returns>
        public static async Task<List<Result>> FlatBlobListing(IAsyncEnumerator<Page<BlobItem>> enumerator, Source source, List<Result> blobsList, bool authSas)
        {
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var blobItems = enumerator.Current;
                    foreach (var blobItem in blobItems.Values)
                    {
                        var blob = authSas ? new BlobClient(new Uri($"{source.Uri}/{source.ContainerName}/{blobItem.Name}?")) : new BlobClient(source.ConnectionString, source.ContainerName, blobItem.Name);
                        blobsList.Add(new Result
                        {
                            BlobType = blobItem.Properties.BlobType.ToString(),
                            Uri = blob.Uri.ToString(),
                            Name = blob.Name,
                            ETag = blobItem.Properties.ETag.ToString()
                        });
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Check authentication information." + ex.ToString());
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
            return blobsList;
        }

        /// <summary>
        ///  List blobs hierarchically.
        /// </summary>
        /// <param name="source">Source values.</param>
        /// <param name="blobsList">List of blobs in container.</param>
        /// <param name="authSas">Authentication method.</param>
        /// <returns>List of blobs in container with hierarchical structure.</returns>
        public static async Task<List<Result>> ListBlobsHierarchy(IAsyncEnumerator<Page<BlobHierarchyItem>> enumerator, Source source, List<Result> blobsList, bool authSas)
        {
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var blobItems = enumerator.Current;
                    foreach (var blobItem in blobItems.Values)
                    {
                        if (blobItem.IsBlob)
                        {
                            var blob = authSas ? new BlobClient(new Uri($"{source.Uri}/{source.ContainerName}/{blobItem.Blob.Name}?")) 
                                    : new BlobClient(source.ConnectionString, source.ContainerName, blobItem.Blob.Name);

                            blobsList.Add(new Result
                            {
                                BlobType = blobItem.Blob.Properties.BlobType.ToString(),
                                Uri = blob.Uri.ToString(),
                                Name = blob.Name,
                                ETag = blobItem.Blob.Properties.ETag.ToString()
                            });
                        }
                        else
                        {
                            blobsList.Add(new Result
                            {
                                BlobType = "Directory",
                                Uri = $"{source.Uri}/{blobItem.Prefix}",
                                Name = blobItem.Prefix,
                                ETag = null
                            });
                        }
                    }
                }
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return blobsList;
        }
    }
}
