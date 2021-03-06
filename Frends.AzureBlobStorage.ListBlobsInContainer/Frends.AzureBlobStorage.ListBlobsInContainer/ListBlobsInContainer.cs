using System;
using System.ComponentModel;
using Azure.Storage.Blobs;
using Azure;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.Threading;

namespace Frends.AzureBlobStorage.ListBlobsInContainer
{
    public class AzureBlobStorage
    {
        /// <summary>
        /// List blobs and subdirectories in Azure Storage container in flat or hierarchical listing structure.
        /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.AzureBlobStorage.ListBlobsInContainer)
        /// </summary>
        /// <param name="source">Source connection parameters.</param>
        /// <param name="options">Options for the task</param>
        /// <returns>object { string Type, string Name, string URI, string ETag }</returns>
        public static async Task<Result> ListBlobsInContainer([PropertyTab] Source source, [PropertyTab] Options options, CancellationToken cancellationToken)
        {
            var authSas = source.AuthenticationMethod.Equals(AuthenticationMethod.SASToken) ? true : false;
            var blobContainerClient = CreateBlobContainerClient(source);


            if (options.ListingStructure is ListingStructure.Flat)
            {
                var enumerable = blobContainerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, string.IsNullOrWhiteSpace(options.Prefix) ? null : options.Prefix, cancellationToken).AsPages();
                var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);
                var flatBlobListing = await FlatBlobListing(enumerator, source, authSas, cancellationToken);
                
                return new Result { BlobList = flatBlobListing };

            }
            else
            {
                var enumerable = blobContainerClient.GetBlobsByHierarchyAsync(BlobTraits.None, BlobStates.None, "/", string.IsNullOrWhiteSpace(options.Prefix) ? null : options.Prefix, cancellationToken).AsPages();
                var enumerator = enumerable.GetAsyncEnumerator(cancellationToken);
                var hierarchyListing = await ListBlobsHierarchy(enumerator, source, authSas, cancellationToken);
                return new Result { BlobList = hierarchyListing };
            }
        }


        private static BlobContainerClient CreateBlobContainerClient(Source source)
        {
            BlobContainerClient blobContainerClient = null;

            switch (source.AuthenticationMethod)
            {
                case AuthenticationMethod.ConnectionString:
                    if (string.IsNullOrWhiteSpace(source.ConnectionString))
                        throw new Exception("Connection string required.");
                    blobContainerClient = new BlobContainerClient(source.ConnectionString, source.ContainerName);
                    break;

                case AuthenticationMethod.SASToken:
                    if (string.IsNullOrWhiteSpace(source.SASToken) || string.IsNullOrWhiteSpace(source.URI))
                        throw new Exception("SAS Token and URI required.");
                    blobContainerClient = new BlobContainerClient(new Uri($"{source.URI}/{source.ContainerName}?"), new AzureSasCredential(source.SASToken));
                    break;
            }

            return blobContainerClient;
        }


        private static async Task<List<BlobData>> FlatBlobListing(IAsyncEnumerator<Page<BlobItem>> enumerator, Source source, bool authSas, CancellationToken cancellationToken)
        {
            var blobListing = new List<BlobData>();
            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var blobItems = enumerator.Current;
                    foreach (var blobItem in blobItems.Values)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var blob = authSas ? new BlobClient(new Uri($"{source.URI}/{source.ContainerName}/{blobItem.Name}?")) : new BlobClient(source.ConnectionString, source.ContainerName, blobItem.Name);
                        blobListing.Add(new BlobData
                        {
                            Type = blobItem.Properties.BlobType.ToString(),
                            URI = blob.Uri.ToString(),
                            Name = blob.Name,
                            ETag = blobItem.Properties.ETag.ToString()
                        }) ;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Check authentication information." + ex.ToString());
            }
            catch (OperationCanceledException ex)
            {
                throw new Exception("Operation cancelled." + ex.ToString());
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
            return blobListing;
        }


        private static async Task<List<BlobData>> ListBlobsHierarchy(IAsyncEnumerator<Page<BlobHierarchyItem>> enumerator, Source source, bool authSas, CancellationToken cancellationToken)
        {
            var blobListing = new List<BlobData>();

            try
            {
                while (await enumerator.MoveNextAsync())
                {
                    var blobItems = enumerator.Current;
                    foreach (var blobItem in blobItems.Values)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (blobItem.IsBlob)
                        {
                            var blob = authSas ? new BlobClient(new Uri($"{source.URI}/{source.ContainerName}/{blobItem.Blob.Name}?")) 
                                    : new BlobClient(source.ConnectionString, source.ContainerName, blobItem.Blob.Name);

                            blobListing.Add(new BlobData
                            {
                                Type = blobItem.Blob.Properties.BlobType.ToString(),
                                URI = blob.Uri.ToString(),
                                Name = blobItem.Blob.Name,
                                ETag = blobItem.Blob.Properties.ETag.ToString()
                            });
                        }
                        else
                        {
                            var blob = authSas ? new BlobContainerClient(new Uri($"{source.URI}/{source.ContainerName}?"))
                                    : new BlobContainerClient(source.ConnectionString, source.ContainerName);

                            blobListing.Add(new BlobData
                            {
                                Type = "Directory",
                                URI = $"{blob.Uri}/{blobItem.Prefix}",
                                Name = blobItem.Prefix,
                                ETag = null
                            });
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new Exception("Check authentication information." + ex.ToString());
            }
            catch (OperationCanceledException ex)
            {
                throw new Exception("Operation cancelled." + ex.ToString());
            }
            finally
            {
                await enumerator.DisposeAsync();
            }

            return blobListing;
        }
    }
}
