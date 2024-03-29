﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Azure.Storage.Blobs;
using Frends.AzureBlobStorage.DownloadBlob.Definitions;
using Azure.Identity;

namespace Frends.AzureBlobStorage.DownloadBlob;

/// <summary>
/// Azure Blob Storage task.
/// </summary>
public static class AzureBlobStorage
{
    /// <summary>
    /// Downloads Blob from Azure Blob Storage.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.AzureBlobStorage.DownloadBlob)
    /// </summary>
    /// <param name="source">Information about which Blob to download.</param>
    /// <param name="destination">Information about the download destination.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this task.</param>
    /// <returns>Object { string FileName, string Directory, string FullPath}</returns>
    public static async Task<Result> DownloadBlob([PropertyTab] Source source, [PropertyTab] Destination destination, CancellationToken cancellationToken)
    {
        try
        {
            //var blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
            var blob = GetBlobClient(source);
            var fullDestinationPath = Path.Combine(destination.Directory, source.BlobName);
            var fileName = source.BlobName.Split('.')[0];
            var fileExtension = "";

            if (source.BlobName.Split('.').Length > 1)
            {
                fileName = string.Join(".", source.BlobName.Split('.').Take(source.BlobName.Split('.').Length - 1).ToArray());
                fileExtension = "." + source.BlobName.Split('.').Last();
            }

            if (destination.FileExistsOperation == FileExistsAction.Error && File.Exists(fullDestinationPath))
                throw new IOException("File already exists in destination path. Please delete the existing file or change the \"file exists operation\" to OverWrite.");

            if (destination.FileExistsOperation == FileExistsAction.Rename && File.Exists(fullDestinationPath))
            {
                var increment = 1;
                var incrementedFileName = $"{fileName}({increment}){fileExtension}";

                while (File.Exists(Path.Combine(destination.Directory, incrementedFileName)))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    increment++;
                    incrementedFileName = $"{fileName}({increment}){fileExtension}";
                }

                fullDestinationPath = Path.Combine(destination.Directory, incrementedFileName);
                fileName = incrementedFileName;
                await blob.DownloadToAsync(fullDestinationPath, cancellationToken);
            }
            else
            {
                await blob.DownloadToAsync(fullDestinationPath, cancellationToken);
            }

            if (!string.IsNullOrEmpty(source.Encoding))
                CheckAndFixFileEncoding(fullDestinationPath, destination.Directory, fileExtension, source.Encoding);

            return new Result(fileName, destination.Directory, fullDestinationPath);
        }
        catch (Exception ex)
        {
            throw new Exception($"DownloadBlob error: {ex}");
        }
    }

    private static void CheckAndFixFileEncoding(string fullPath, string directory, string fileExtension, string targetEncoding)
    {
        string encoding;

        using (var reader = new StreamReader(fullPath, true))
        {
            reader.Read();
            encoding = reader.CurrentEncoding.BodyName;
        }
        if (targetEncoding.ToLower() != encoding)
        {
            Encoding newEncoding;
            try
            {
                newEncoding = CodePagesEncodingProvider.Instance.GetEncoding(targetEncoding.ToLower()) != null
                    ? CodePagesEncodingProvider.Instance.GetEncoding(targetEncoding.ToLower())
                    : Encoding.GetEncoding(targetEncoding.ToLower());
            }
            catch (Exception)
            {
                throw new Exception($"Provided encoding {targetEncoding} is not supported. Please check supported encodings from Encoding-option.");
            }
            var tempFilePath = Path.Combine(directory, "encodingTemp" + fileExtension);
            using (var sr = new StreamReader(fullPath, true))
            using (var sw = new StreamWriter(tempFilePath, false, newEncoding))
            {
                var line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                    sw.WriteLine(line);
            }
            File.Delete(fullPath);
            File.Copy(tempFilePath, fullPath);
            File.Delete(tempFilePath);
        }
    }

    private static BlobClient GetBlobClient(Source source)
    {
        switch (source.ConnectionMethod)
        {
            case ConnectionMethod.ConnectionString:
                return new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
            case ConnectionMethod.OAuth2:
                var credentials = new ClientSecretCredential(source.TenantID, source.ApplicationID, source.ClientSecret, new ClientSecretCredentialOptions());
                var url = new Uri($"https://{source.StorageAccountName}.blob.core.windows.net/{source.ContainerName.ToLower()}/{source.BlobName}");
                return new BlobClient(url, credentials);
            default: throw new NotSupportedException();
        }
    }
}