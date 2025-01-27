﻿using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using MimeMapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob;

/// <summary>
/// Azure Blob Storage Task.
/// </summary>
public class AzureBlobStorage
{
    /// <summary>
    /// Frends Task to upload blobs to Azure Blob Storage.
    /// [Documentation](https://tasks.frends.com/tasks/frends-tasks/Frends.AzureBlobStorage.UploadBlob)
    /// </summary>
    /// <param name="source">Source parameters.</param>
    /// <param name="destination">Destination parameters.</param>
    /// <param name="options">Optional parameters.</param>
    /// <param name="cancellationToken">Token generated by Frends to stop this Task.</param>
    /// <returns>Object { bool Success, Dictionary&lt;string, string&gt; Data }</returns>
    public static async Task<Result> UploadBlob([PropertyTab] Source source, [PropertyTab] Destination destination, [PropertyTab] Options options, CancellationToken cancellationToken)
    {
        var results = new Dictionary<string, string>();

        var fi = string.IsNullOrEmpty(source.SourceFile) ? null : new FileInfo(source.SourceFile);
        var handledFile = string.Empty;

        try
        {
            CheckParameters(destination, source);
            var blobName = string.Empty;

            if (destination.CreateContainerIfItDoesNotExist && (destination.ConnectionMethod is ConnectionMethod.ConnectionString || destination.ConnectionMethod is ConnectionMethod.OAuth2))
                await CreateContainerIfItDoesNotExist(destination, destination.ContainerName.ToLower(), cancellationToken);

            switch (source.SourceType)
            {
                case UploadSourceType.File:
                    if (fi == null)
                        throw new FileNotFoundException($"Source file '{source.SourceFile}' was empty.");
                    blobName = fi.Name;
                    if (!string.IsNullOrWhiteSpace(source.BlobName) || source.Compress)
                        blobName = RenameFile(!string.IsNullOrEmpty(source.BlobName) ? source.BlobName : fi.Name, source.Compress, fi);
                    results.Add(source.SourceFile, await HandleUpload(source, destination, options, fi, blobName, cancellationToken));
                    break;
                case UploadSourceType.Directory:
                    var dir = string.IsNullOrEmpty(source.SourceDirectory) ? null : source.SourceDirectory;
                    foreach (var file in Directory.GetFiles(dir, string.IsNullOrEmpty(source.SearchPattern) ? "*.*" : source.SearchPattern, SearchOption.AllDirectories)
                        .Select(e => new FileInfo(e)))
                    {
                        var fileName = file.Name;
                        if (source.Compress)
                            fileName = RenameFile(fileName, source.Compress, file);

                        var parentDirectory = Path.GetFileName(Path.GetDirectoryName(file.ToString()));
                        var withDir = string.IsNullOrEmpty(source.BlobFolderName)
                            ? Path.Combine(parentDirectory, fileName)
                            : Path.Combine(source.BlobFolderName, fileName);

                        blobName = withDir.Replace("\\", "/");

                        results.Add(file.FullName, await HandleUpload(source, destination, options, file, blobName, cancellationToken));
                        handledFile = file.FullName;
                    }

                    if (!results.Any())
                    {
                        if (options.ThrowErrorOnFailure)
                            throw new Exception(@$"No files were found in the directory {dir}.");
                        else
                            results.Add(null, @$"No files were found in the directory {dir}.");
                    }
                    break;
                default:
                    throw new Exception("Invalid source.");
            }
        }
        catch (Exception ex)
        {
            var error = new Dictionary<string, string>();
            if (source.SourceType is UploadSourceType.File)
            {
                if (options.ThrowErrorOnFailure)
                    throw new Exception("An exception occured.", ex);
                else
                {
                    if (fi == null)
                        error.Add(string.Empty, $@"An exception occured. {ex}");
                    else
                        error.Add(fi.FullName, $@"An exception occured. {ex}");
                    return new Result(false, error);
                }
            }
            else
            {
                if (options.ThrowErrorOnFailure)
                    throw new Exception($@"An exception occured while uploading directory. Last handled file: {handledFile}", ex);
                else
                {
                    error.Add(string.Empty, $@"An exception occured while uploading directory. Last handled file: {handledFile}. {ex}");
                    return new Result(false, error);
                }
            }
        }

        return new Result(true, results);
    }

    private static async Task<string> HandleUpload(Source source, Destination destination, Options options, FileInfo fi, string blobName, CancellationToken cancellationToken)
    {
        blobName = string.IsNullOrEmpty(source.BlobName) ? blobName : source.BlobName;

        var contentType = string.IsNullOrEmpty(destination.ContentType) ? MimeUtility.GetMimeMapping(fi.Name) : destination.ContentType;
        var encoding = GetEncoding(destination.Encoding, destination.FileEncodingString, destination.EnableBOM);

        var tags = source.Tags != null ? source.Tags.ToDictionary(tag => tag.Name, tag => tag.Value) : new Dictionary<string, string>();

        var credentials = destination.ConnectionMethod is ConnectionMethod.OAuth2 ? new ClientSecretCredential(destination.TenantID, destination.ApplicationID, destination.ClientSecret, new ClientSecretCredentialOptions()) : null;

        BlobContainerClient containerClient = null;

        if (destination.ConnectionMethod is ConnectionMethod.ConnectionString)
            containerClient = new BlobContainerClient(destination.ConnectionString, destination.ContainerName.ToLower());
        else if (destination.ConnectionMethod is ConnectionMethod.SASToken)
            containerClient = new BlobContainerClient(new Uri($"{destination.Uri}/{destination.ContainerName}?"), new AzureSasCredential(destination.SASToken));

        var overwrite = destination.HandleExistingFile == HandleExistingFile.Overwrite;

        switch (destination.BlobType)
        {
            case AzureBlobType.Append:
                try
                {
                    AppendBlobClient appendBlobClient = destination.ConnectionMethod is ConnectionMethod.OAuth2 ? new AppendBlobClient(new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{blobName}"), credentials) : containerClient.GetAppendBlobClient(blobName);

                    var exists = false;
                    exists = await appendBlobClient.ExistsAsync(cancellationToken);

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Error)
                    {
                        if (!options.ThrowErrorOnFailure)
                            return @$"Blob {blobName} already exists.";
                        else
                            throw new Exception(@$"Blob {blobName} already exists.");
                    }

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Overwrite)
                    {
                        await appendBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);
                        exists = false;
                    }

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Append)
                        fi = await AppendAny(appendBlobClient, blobName, source.SourceFile, cancellationToken);

                    if (fi != null)
                    {
                        var appendBlobCreateOptions = new AppendBlobCreateOptions
                        {
                            HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName },
                            Tags = tags.Count > 0 ? tags : null
                        };

                        await appendBlobClient.CreateAsync(appendBlobCreateOptions, cancellationToken);
                        using var appendGetStream = new MemoryStream(GetBytes(false, true, encoding, fi));
                        await appendBlobClient.AppendBlockAsync(appendGetStream, null, null, null, cancellationToken);
                    }

                    return appendBlobClient.Uri.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception($"UploadBlob (Append): An error occured while uploading {blobName}.", ex);
                }

            case AzureBlobType.Block:
                try
                {
                    var blobname = blobName;
                    BlobClient blobClient = destination.ConnectionMethod is ConnectionMethod.OAuth2 ? new BlobClient(new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{blobName}"), credentials) : containerClient.GetBlobClient(blobName);

                    var exists = await blobClient.ExistsAsync(cancellationToken);

                    if (exists.Value && destination.HandleExistingFile is HandleExistingFile.Error)
                    {
                        if (!options.ThrowErrorOnFailure)
                            return @$"Blob {blobName} already exists.";
                        else
                            throw new Exception(@$"Blob {blobName} already exists.");
                    }

                    var blobUploadOptions = new BlobUploadOptions
                    {
                        Conditions = overwrite ? null : new BlobRequestConditions { IfNoneMatch = new ETag("*") },
                        TransferOptions = new StorageTransferOptions { MaximumConcurrency = destination.ParallelOperations },
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName },
                        Tags = tags.Count > 0 ? tags : null,
                    };

                    if (exists.Value && destination.HandleExistingFile is HandleExistingFile.Overwrite)
                        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

                    if (exists.Value && destination.HandleExistingFile is HandleExistingFile.Append)
                    {
                        fi = await AppendAny(blobClient, blobName, source.SourceFile, cancellationToken);
                        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);
                    }

                    await blobClient.UploadAsync(BinaryData.FromBytes(GetBytes(source.Compress, source.ContentsOnly, encoding, fi)), blobUploadOptions, cancellationToken);

                    //Delete temp file
                    if (File.Exists(fi.FullName) && Path.GetDirectoryName(fi.FullName) != Path.GetDirectoryName(source.SourceFile) && Path.GetDirectoryName(fi.FullName) != source.SourceDirectory)
                        fi.Delete();

                    return blobClient.Uri.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception($"UploadBlob (Block): An error occured while uploading {blobName}.", ex);
                }

            case AzureBlobType.Page:
                try
                {
                    PageBlobClient pageBlobClient = destination.ConnectionMethod is ConnectionMethod.OAuth2 ? new PageBlobClient(new Uri($"{destination.Uri}/{destination.ContainerName.ToLower()}/{blobName}"), credentials) : containerClient.GetPageBlobClient(blobName);

                    var origSize = 0;
                    var exists = false;
                    exists = await pageBlobClient.ExistsAsync(cancellationToken);

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Error)
                    {
                        if (options.ThrowErrorOnFailure)
                            throw new Exception(@$"Blob {blobName} already exists.");
                        else
                            return @$"Blob {blobName} already exists.";
                    }

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Overwrite)
                    {
                        await pageBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);
                        exists = false;
                    }

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Append)
                    {
                        origSize = pageBlobClient.PageBlobPageBytes;
                        fi = await AppendAny(pageBlobClient, blobName, source.SourceFile, cancellationToken);
                    }

                    var bytesMissing = 0;

                    var requiredSize = destination.PageMaxSize < 512 ? 512 : destination.PageMaxSize;
                    var fiMinLenght = new FileInfo(fi.FullName).Length;
                    while ((fiMinLenght + bytesMissing) % 512 != 0)
                        bytesMissing++;

                    if (requiredSize < (fiMinLenght + bytesMissing))
                        requiredSize = fiMinLenght + bytesMissing;

                    if (requiredSize > 8796093022208) //Handle over 8 TB
                        throw new Exception($"UploadPageBlob, blob {blobName}: Required minimum size of Page is over 8 TB and size must be multiple of 512. Required size that is multiple of 512 is {requiredSize} bytes.");

                    // Fill file until size is multiple of 512.
                    if (destination.ResizeFile && bytesMissing > 0)
                    {
                        using var fileStream = new FileStream(fi.FullName, FileMode.Append);

                        fileStream.Write(new byte[bytesMissing], 0, bytesMissing);

                        if (exists)
                            await pageBlobClient.ResizeAsync(fiMinLenght + bytesMissing, cancellationToken: cancellationToken);
                    }

                    using var pageGetStream = new MemoryStream(GetBytes(false, true, encoding, fi));

                    if (!exists)
                        await pageBlobClient.CreateAsync(requiredSize, cancellationToken: cancellationToken);

                    await pageBlobClient.UploadPagesAsync(pageGetStream, offset: destination.PageOffset == -1 ? origSize : destination.PageOffset, cancellationToken: cancellationToken);

                    if (Path.GetDirectoryName(fi.FullName) != Path.GetDirectoryName(source.SourceFile) && Path.GetDirectoryName(fi.FullName) != source.SourceDirectory)
                        fi.Delete();

                    return pageBlobClient.Uri.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception($"UploadBlob (Page): An error occured while uploading {blobName}. {ex}");
                }

            default: throw new Exception(@$"HandleUpload: An error occured while uploading {blobName}. Missing Azure Blob type.");
        }
    }

    private static string RenameFile(string renameTo, bool compress, FileInfo fi)
    {
        try
        {
            string fileName;

            if (string.IsNullOrEmpty(renameTo) && compress)
                fileName = fi.Name + ".gz";
            else if (string.IsNullOrEmpty(renameTo))
                fileName = fi.Name;
            else
                fileName = renameTo;

            return fileName;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while renaming file. {ex}");
        }
    }

    private static async Task CreateContainerIfItDoesNotExist(Destination destination, string containerName, CancellationToken cancellationToken)
    {
        try
        {
            BlobServiceClient blobServiceClient;
            if (destination.ConnectionMethod is ConnectionMethod.ConnectionString)
            {
                blobServiceClient = new BlobServiceClient(destination.ConnectionString);
            }
            else if (destination.ConnectionMethod is ConnectionMethod.SASToken)
            {
                var serviceURI = new Uri($"{destination.Uri}");
                blobServiceClient = new BlobServiceClient(serviceURI, new AzureSasCredential(destination.SASToken));
            }
            else
            {
                var serviceURI = new Uri($"{destination.Uri}");
                var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID, destination.ClientSecret, new ClientSecretCredentialOptions());
                blobServiceClient = new BlobServiceClient(serviceURI, credentials);
            }

            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while checking if container exists or while creating a new container. {ex}");
        }
    }

    private static async Task<FileInfo> AppendAny(BlobBaseClient blobClient, string blobName, string sourceFile, CancellationToken cancellationToken)
    {
        try
        {
            BlobProperties blobProperties = null;

            blobProperties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            if (blobProperties == null)
                throw new Exception("Blob properties couldn't be fetched.");

            //Block and Page blobs need to be downloaded and handled in temp because file size can be too large for memory stream.
            if (blobProperties.BlobType.Equals(BlobType.Append))
            {
                var appendBlobMaxAppendBlockBytes = ((AppendBlobClient)blobClient).AppendBlobMaxAppendBlockBytes;

                using var file = File.OpenRead(sourceFile);
                {
                    int bytesRead;
                    var buffer = new byte[appendBlobMaxAppendBlockBytes];
                    while ((bytesRead = await file.ReadAsync(buffer, cancellationToken)) > 0)
                    {
                        var newArray = new Span<byte>(buffer, 0, bytesRead).ToArray();
                        using Stream stream = new MemoryStream(newArray) { Position = 0 };
                        await ((AppendBlobClient)blobClient).AppendBlockAsync(stream, cancellationToken: cancellationToken);
                    }
                }

                return null;
            }
            else
            {
                var tempFile = Path.Combine(Path.GetTempPath(), blobName);
                await blobClient.DownloadToAsync(tempFile, cancellationToken);

                using var sourceData = new StreamReader(sourceFile);
                using var destinationFile = File.AppendText(tempFile);
                var line = await sourceData.ReadLineAsync();
                await destinationFile.WriteAsync(line);

                return new FileInfo(tempFile);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"AppendAny: An error occured while appending file. {ex}");
        }
    }

    private static byte[] GetBytes(bool compress, bool fromString, Encoding encoding, FileInfo file)
    {
        var fileStream = File.OpenRead(file.FullName);

        try
        {
            if (!compress)
            {
                using var reader = new StreamReader(fileStream, encoding);
                return encoding.GetBytes(reader.ReadToEnd());
            }

            using var outStream = new MemoryStream();
            using (var gzip = new GZipStream(outStream, CompressionMode.Compress, true))
            {
                if (!fromString)
                {
                    fileStream.CopyTo(gzip);
                }
                else
                {
                    using var reader = new StreamReader(fileStream, encoding);
                    var content = reader.ReadToEnd();
                    using var encodedMemory = new MemoryStream(encoding.GetBytes(content));
                    encodedMemory.CopyTo(gzip);
                }
            }

            return outStream.ToArray();
        }
        finally
        {
            fileStream.Dispose();
        }
    }

    private static Encoding GetEncoding(FileEncoding encoding, string encodingString, bool enableBom)
    {
        return encoding switch
        {
            FileEncoding.UTF8 => enableBom ? new UTF8Encoding(true) : new UTF8Encoding(false),
            FileEncoding.ASCII => new ASCIIEncoding(),
            FileEncoding.Default => Encoding.Default,
            FileEncoding.WINDOWS1252 => CodePagesEncodingProvider.Instance.GetEncoding("windows-1252"),
            FileEncoding.Other => CodePagesEncodingProvider.Instance.GetEncoding(encodingString),
            _ => throw new ArgumentOutOfRangeException($"Unknown Encoding type: '{encoding}'."),
        };
    }

    private static void CheckParameters(Destination destination, Source source)
    {
        if (source.SourceType is UploadSourceType.Directory && !string.IsNullOrEmpty(source.SourceDirectory) && !Directory.Exists(source.SourceDirectory))
            throw new Exception(@$"Source directory {source.SourceDirectory} doesn't exists.");
        if (source.SourceType is UploadSourceType.Directory && (string.IsNullOrEmpty(source.SourceDirectory) || !Directory.EnumerateFileSystemEntries(source.SourceDirectory).Any()))
            throw new Exception(@$"Source.SourceDirectory value is empty.");
        if (source.SourceType is UploadSourceType.File && string.IsNullOrEmpty(source.SourceFile))
            throw new Exception("Source.SourceFile value is empty.");
        if (source.SourceType is UploadSourceType.File && !File.Exists(source.SourceFile))
            throw new Exception("Source.SourceFile not found.");
        if (destination.ConnectionMethod is ConnectionMethod.OAuth2 && (string.IsNullOrEmpty(destination.ApplicationID) || string.IsNullOrEmpty(destination.ClientSecret) || string.IsNullOrEmpty(destination.TenantID) || string.IsNullOrEmpty(destination.Uri)))
            throw new Exception("Destination.StorageAccountName, Destination.ClientSecret, Destination.ApplicationID and Destination.TenantID parameters can't be empty when Destination.ConnectionMethod = OAuth.");
        if (destination.ConnectionMethod is ConnectionMethod.ConnectionString && string.IsNullOrEmpty(destination.ConnectionString))
            throw new Exception("Destination.ConnectionString parameter can't be empty when Destination.ConnectionMethod = ConnectionString.");
        if (destination.ConnectionMethod is ConnectionMethod.SASToken && (string.IsNullOrEmpty(destination.Uri) || string.IsNullOrEmpty(destination.SASToken)))
            throw new Exception("Destination.SASToken and Destination.URI parameters can't be empty when Destination.ConnectionMethod = SASToken.");
        if (string.IsNullOrEmpty(destination.ContainerName))
            throw new Exception("Destination.ContainerName parameter can't be empty.");
    }
}