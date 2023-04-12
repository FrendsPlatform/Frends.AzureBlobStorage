﻿using Azure.Identity;
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
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob;

/// <summary>
/// Azure Blob Storage Task.
/// </summary>
public class AzureBlobStorage
{
    /// For mem cleanup.
    static AzureBlobStorage()
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentContext = AssemblyLoadContext.GetLoadContext(currentAssembly);
        if (currentContext != null)
            currentContext.Unloading += OnPluginUnloadingRequested;
    }

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
        var fi = string.IsNullOrWhiteSpace(source.SourceFile) ? null : new FileInfo(source.SourceFile);
        var handledFile = "";

        try
        {
            CheckParameters(destination, source);
            var blobName = "";

            if (destination.CreateContainerIfItDoesNotExist && destination.ConnectionMethod is ConnectionMethod.ConnectionString)
                await CreateContainerIfItDoesNotExist(destination.ConnectionString, destination.ContainerName.ToLower(), cancellationToken);

            switch (source.SourceType)
            {
                case UploadSourceType.File:
                    blobName = fi.Name;
                    if (!string.IsNullOrWhiteSpace(source.BlobName) || source.Compress)
                        blobName = RenameFile(!string.IsNullOrWhiteSpace(source.BlobName) ? source.BlobName : fi.Name, source.Compress, fi);
                    results.Add(source.SourceFile, await HandleUpload(source, destination, options, fi, blobName, cancellationToken));
                    break;
                case UploadSourceType.Directory:
                    var dir = string.IsNullOrWhiteSpace(source.SourceDirectory) ? null : source.SourceDirectory;
                    var files = Directory.GetFiles(dir, string.IsNullOrWhiteSpace(source.SearchPattern) ? "*.*" : source.SearchPattern, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        for (var i = 0; i < files.Length; i++)
                        {
                            var file = new FileInfo(files[i]);
                            var fileName = file.Name;

                            if (source.Compress)
                                fileName = RenameFile(fileName, source.Compress, file);

                            var parentDirectory = Path.GetFileName(Path.GetDirectoryName(file.ToString()));
                            var withDir = "";
                            if (string.IsNullOrWhiteSpace(source.BlobFolderName))
                                withDir = Path.Combine(parentDirectory, fileName);
                            else
                                withDir = Path.Combine(source.BlobFolderName, fileName);

                            blobName = withDir.Replace("\\", "/");

                            results.Add(file.FullName, await HandleUpload(source, destination, options, file, blobName, cancellationToken));
                            handledFile = file.FullName;
                        }
                    }
                    else
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
            if (source.SourceType is UploadSourceType.Directory)
            {
                if (options.ThrowErrorOnFailure)
                    throw new Exception($@"An exception occured while uploading directory. Last handled file: {handledFile}", ex);
                else
                {
                    error.Add(null, $@"An exception occured while uploading directory. Last handled file: {handledFile}. {ex}");
                    return new Result(false, error);
                }
            }
            else
            {
                if (options.ThrowErrorOnFailure)
                    throw new Exception("An exception occured.", ex);
                else
                {
                    error.Add(fi.FullName, $@"An exception occured. {ex}");
                    return new Result(false, error);
                }
            }
        }

        return new Result(true, results);
    }

    private static async Task<string> HandleUpload(Source source, Destination destination, Options options, FileInfo fi, string blobName, CancellationToken cancellationToken)
    {
        var result = new Dictionary<string, string>();
        blobName = string.IsNullOrWhiteSpace(source.BlobName) ? blobName : source.BlobName;

        var contentType = string.IsNullOrWhiteSpace(destination.ContentType) ? MimeUtility.GetMimeMapping(fi.Name) : destination.ContentType;
        var encoding = GetEncoding(destination.FileEncoding);
        var progressHandler = new Progress<long>(progress => { Console.WriteLine("Bytes uploaded: {0}", progress); });

        var tags = new Dictionary<string, string>();
        if (source.Tags != null && source.Tags.Length > 0)
            foreach (var tag in source.Tags)
                tags.Add(tag.Name, tag.Value);

        switch (destination.BlobType)
        {
            case AzureBlobType.Append:
                try
                {
                    AppendBlobClient appendBlobClient;
                    if (destination.ConnectionMethod is ConnectionMethod.ConnectionString)
                        appendBlobClient = new AppendBlobClient(destination.ConnectionString, destination.ContainerName.ToLower(), blobName);
                    else
                    {
                        var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID, destination.ClientSecret, new ClientSecretCredentialOptions());
                        var url = new Uri($"https://{destination.StorageAccountName}.blob.core.windows.net/{destination.ContainerName.ToLower()}/{blobName}");
                        appendBlobClient = new AppendBlobClient(url, credentials);
                    }

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
                        fi = await AppendAny(null, appendBlobClient, null, blobName, source.SourceFile, cancellationToken);

                    if (fi != null)
                    {
                        var appendBlobCreateOptions = new AppendBlobCreateOptions
                        {
                            HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName },
                            Tags = tags.Count > 0 ? tags : null
                        };

                        await appendBlobClient.CreateAsync(appendBlobCreateOptions, cancellationToken);
                        using var appendGetStream = GetStream(false, true, encoding, fi);
                        await appendBlobClient.AppendBlockAsync(appendGetStream, null, null, progressHandler, cancellationToken);
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
                    BlobClient blobClient;
                    if (destination.ConnectionMethod is ConnectionMethod.ConnectionString)
                        blobClient = new BlobClient(destination.ConnectionString, destination.ContainerName.ToLower(), blobName);
                    else
                    {
                        var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID, destination.ClientSecret, new ClientSecretCredentialOptions());
                        var url = new Uri($"https://{destination.StorageAccountName}.blob.core.windows.net/{destination.ContainerName.ToLower()}/{blobName}");
                        blobClient = new BlobClient(url, credentials);
                    }

                    var exists = await blobClient.ExistsAsync(cancellationToken);

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Error)
                    {
                        if (!options.ThrowErrorOnFailure)
                            return @$"Blob {blobName} already exists.";
                        else
                            throw new Exception(@$"Blob {blobName} already exists.");
                    }

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Overwrite)
                        await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.None, null, cancellationToken);

                    if (exists && destination.HandleExistingFile is HandleExistingFile.Append)
                        fi = await AppendAny(blobClient, null, null, blobName, source.SourceFile, cancellationToken);

                    var blobUploadOptions = new BlobUploadOptions
                    {
                        ProgressHandler = progressHandler,
                        TransferOptions = new StorageTransferOptions { MaximumConcurrency = destination.ParallelOperations },
                        HttpHeaders = new BlobHttpHeaders { ContentType = contentType, ContentEncoding = source.Compress ? "gzip" : encoding.WebName },
                        Tags = tags.Count > 0 ? tags : null
                    };

                    using (var stream = GetStream(source.Compress, source.ContentsOnly, encoding, fi))
                    {
                        await blobClient.UploadAsync(stream, blobUploadOptions, cancellationToken);
                    }

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
                    PageBlobClient pageBlobClient;
                    if (destination.ConnectionMethod is ConnectionMethod.ConnectionString)
                        pageBlobClient = new PageBlobClient(destination.ConnectionString, destination.ContainerName.ToLower(), blobName);
                    else
                    {
                        var credentials = new ClientSecretCredential(destination.TenantID, destination.ApplicationID, destination.ClientSecret, new ClientSecretCredentialOptions());
                        var url = new Uri($"https://{destination.StorageAccountName}.blob.core.windows.net/{destination.ContainerName.ToLower()}/{blobName}");
                        pageBlobClient = new PageBlobClient(url, credentials);
                    }

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
                        fi = await AppendAny(null, null, pageBlobClient, blobName, source.SourceFile, cancellationToken);
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
                        using (var fileStream = new FileStream(fi.FullName, FileMode.Append))
                        {
                            fileStream.Write(new byte[bytesMissing], 0, bytesMissing);
                        }

                        if (exists)
                            await pageBlobClient.ResizeAsync(fiMinLenght + bytesMissing, cancellationToken: cancellationToken);
                    }

                    using var pageGetStream = GetStream(false, true, encoding, fi);
                    {
                        if (!exists)
                            await pageBlobClient.CreateAsync(requiredSize, cancellationToken: cancellationToken);

                        await pageBlobClient.UploadPagesAsync(pageGetStream, offset: destination.PageOffset == -1 ? origSize : destination.PageOffset, cancellationToken: cancellationToken);
                    }

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
            if (string.IsNullOrWhiteSpace(renameTo) && compress) fileName = fi.Name + ".gz";
            else if (string.IsNullOrWhiteSpace(renameTo)) fileName = fi.Name;
            else fileName = renameTo;
            return fileName;
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while renaming file. {ex}");
        }
    }

    private static async Task CreateContainerIfItDoesNotExist(string connectionString, string containerName, CancellationToken cancellationToken)
    {
        try
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            var container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occured while checking if container exists or while creating a new container. {ex}");
        }
    }

    private static async Task<FileInfo> AppendAny(BlobClient blob, AppendBlobClient appendBlobClient, PageBlobClient pageBlobClient, string blobName, string sourceFile, CancellationToken cancellationToken)
    {
        try
        {
            Task<Azure.Response<BlobProperties>> blobProperties = null;

            if (blob is null && appendBlobClient is null && pageBlobClient is null)
                throw new Exception("AppendAny exception: Client missing.");
            if (blob != null)
                blobProperties = blob.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (appendBlobClient != null)
                blobProperties = appendBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
            if (pageBlobClient != null)
                blobProperties = pageBlobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

            //Block and Page blobs need to be downloaded and handled in temp because file size can be too large for memory stream.
            if (blobProperties.Result.Value.BlobType.Equals(BlobType.Append))
            {
                var appendBlobMaxAppendBlockBytes = appendBlobClient.AppendBlobMaxAppendBlockBytes;

                using var file = File.OpenRead(sourceFile);
                {
                    int bytesRead;
                    var buffer = new byte[appendBlobMaxAppendBlockBytes];
                    while ((bytesRead = await file.ReadAsync(buffer, cancellationToken)) > 0)
                    {
                        var newArray = new Span<byte>(buffer, 0, bytesRead).ToArray();
                        Stream stream = new MemoryStream(newArray) { Position = 0 };
                        await appendBlobClient.AppendBlockAsync(stream, cancellationToken: cancellationToken);
                    }
                }
                file.Close();
                await file.DisposeAsync();

                return null;
            }
            else
            {
                var tempFile = Path.Combine(Path.GetTempPath(), blobName);

                if (blob != null)
                    await blob.DownloadToAsync(tempFile, cancellationToken);
                if (appendBlobClient != null)
                    await appendBlobClient.DownloadToAsync(tempFile, cancellationToken);
                if (pageBlobClient != null)
                    await pageBlobClient.DownloadToAsync(tempFile, cancellationToken);

                using (var sourceData = new StreamReader(sourceFile))
                {
                    using (var destinationFile = File.AppendText(tempFile))
                    {
                        var line = await sourceData.ReadLineAsync();
                        await destinationFile.WriteAsync(line);
                    };
                }

                return new FileInfo(tempFile);
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"AppendAny: An error occured while appending file. {ex}");
        }
    }

    private static Stream GetStream(bool compress, bool fromString, Encoding encoding, FileInfo file)
    {
        var fileStream = File.OpenRead(file.FullName);

        if (!compress && !fromString)
            return fileStream;

        byte[] bytes;

        if (!compress)
        {
            using (var reader = new StreamReader(fileStream, encoding)) bytes = encoding.GetBytes(reader.ReadToEnd());
            return new MemoryStream(bytes);
        }

        using (var outStream = new MemoryStream())
        {
            using (var gzip = new GZipStream(outStream, CompressionMode.Compress))
            {
                if (!fromString) fileStream.CopyTo(gzip);
                else
                    using (var reader = new StreamReader(fileStream, encoding))
                    {
                        var content = reader.ReadToEnd();
                        using var encodedMemory = new MemoryStream(encoding.GetBytes(content));
                        encodedMemory.CopyTo(gzip);
                    }
            }
            bytes = outStream.ToArray();
        }
        fileStream.Close();
        fileStream.Dispose();

        var memStream = new MemoryStream(bytes);
        return memStream;
    }

    private static Encoding GetEncoding(string target)
    {
        return target.ToLower() switch
        {
            "utf-32" => Encoding.UTF32,
            "unicode" => Encoding.Unicode,
            "ascii" => Encoding.ASCII,
            _ => Encoding.UTF8,
        };
    }

    private static void CheckParameters(Destination destination, Source source)
    {
        if (!string.IsNullOrWhiteSpace(source.SourceDirectory) && source.SourceType is UploadSourceType.Directory && !Directory.Exists(source.SourceDirectory))
            throw new Exception(@$"Source directory {source.SourceDirectory} doesn't exists.");
        if (!string.IsNullOrWhiteSpace(source.SourceDirectory) && source.SourceType is UploadSourceType.Directory && !Directory.EnumerateFileSystemEntries(source.SourceDirectory).Any())
            throw new Exception(@$"Source directory {source.SourceDirectory} is empty.");
        if (!string.IsNullOrWhiteSpace(source.SourceFile) && source.SourceType is UploadSourceType.Directory)
            throw new Exception("Source.SourceFile must be empty when Source.SourceType is Directory.");
        if (string.IsNullOrWhiteSpace(source.SourceDirectory) && source.SourceType is UploadSourceType.Directory)
            throw new Exception("Source.SourceDirectory value is empty.");

        if (!string.IsNullOrWhiteSpace(source.SourceFile) && source.SourceType is UploadSourceType.Directory && File.Exists(source.SourceFile))
            throw new Exception(@$"Source file {source.SourceFile} doesn't exists.");
        if (!string.IsNullOrWhiteSpace(source.SourceDirectory) && source.SourceType is UploadSourceType.File)
            throw new Exception("Source.SourceDirectory must be empty when Source.SourceType is Directory.");
        if (string.IsNullOrWhiteSpace(source.SourceFile) && source.SourceType is UploadSourceType.File)
            throw new Exception("Source.SourceFile not found.");
        if (string.IsNullOrWhiteSpace(source.SourceFile) && source.SourceType is UploadSourceType.File)
            throw new Exception("Source.SourceFile not found.");


        if (destination.ConnectionMethod is ConnectionMethod.OAuth2 && (destination.ApplicationID is null || destination.ClientSecret is null || destination.TenantID is null || destination.StorageAccountName is null))
            throw new Exception("Destination.StorageAccountName, Destination.ClientSecret, Destination.ApplicationID and Destination.TenantID parameters can't be empty when Destination.ConnectionMethod = OAuth.");
        if (string.IsNullOrWhiteSpace(destination.ConnectionString) && destination.ConnectionMethod is ConnectionMethod.ConnectionString)
            throw new Exception("Destination.ConnectionString parameter can't be empty when Destination.ConnectionMethod = ConnectionString.");
        if (string.IsNullOrWhiteSpace(destination.ContainerName))
            throw new Exception("Destination.ContainerName parameter can't be empty.");
    }

    private static void OnPluginUnloadingRequested(AssemblyLoadContext obj)
    {
        obj.Unloading -= OnPluginUnloadingRequested;
    }
}