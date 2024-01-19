using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests.lib;

internal class Helper
{
    internal static async Task CreateContainerAndTestFiles(bool delete, string connString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connString);
        var container = blobServiceClient.GetBlobContainerClient(containerName);
        if (delete)
            await container.DeleteIfExistsAsync();
        else
        {
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());

            var files = new List<string>()
            {
                "TestFile.txt", "TestFile2.txt", "Temp/SubFolderFile", "Temp/SubFolderFile2"
            };

            foreach (var file in files)
                await container.UploadBlobAsync(file, new BinaryData(Encoding.UTF32.GetBytes($"This is {file}")));
        }
    }
}

