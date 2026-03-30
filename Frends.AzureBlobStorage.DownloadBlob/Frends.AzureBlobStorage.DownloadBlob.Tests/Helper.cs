using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DownloadBlob.Tests;
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

			var createdDate = DateTimeOffset.UtcNow.AddDays(+1);

			foreach (var file in files)
			{
				createdDate = createdDate.AddDays(-1);
				var blobClient = container.GetBlobClient(file);

				await blobClient.UploadAsync(
					new MemoryStream(Encoding.UTF8.GetBytes($"This is {file}"))
				);
			}
		}
	}
}
