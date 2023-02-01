using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

[TestClass]
public class OAuthUnitTests
{
    private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
    private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
    private readonly string _uri = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_URI");

    [TestInitialize]
    public async Task Init()
    {
        await CreateContainerAndTestFiles(false);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        await CreateContainerAndTestFiles(true);
    }

    [TestMethod]
    public async Task ListBlob_OAuth_ListingStructures()
    {
        var listing = new List<ListingStructure>() { ListingStructure.Flat, ListingStructure.Hierarchical };

        var source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.OAuth2,
            ApplicationID = _appID,
            TenantID = _tenantID,
            ClientSecret = _clientSecret,
            URI = _uri,
            ContainerName = _containerName,
        };

        foreach(var structure in listing) 
        {
            var options = new Options
            {
                Prefix = null,
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(source, options, default);

            if(structure is ListingStructure.Flat)
            {
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile"));
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile2"));
                Assert.IsTrue(result.BlobList.Any(x => x.URI.Contains("/Temp/SubFolderFile")));
                Assert.IsTrue(result.BlobList.Any(x => x.URI.Contains("/Temp/SubFolderFile2")));
            }
            else
            {
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/"));
                Assert.IsTrue(result.BlobList.Any(x => x.URI.Contains("/Temp/")));
            }

            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile2.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.ETag != null));
            Assert.IsTrue(result.BlobList.Any(x => x.Type != null));
        }
    }
    
    [TestMethod]
    public async Task ListBlob_OAuth_Prefix()
    {
        var listing = new List<ListingStructure>() { ListingStructure.Flat, ListingStructure.Hierarchical };

        var source = new Source
        {
            AuthenticationMethod = AuthenticationMethod.OAuth2,
            ApplicationID = _appID,
            TenantID = _tenantID,
            ClientSecret = _clientSecret,
            URI = _uri,
            ContainerName = _containerName,
        };

        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = "Tes",
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(source, options, default);

            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile"));
            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile2"));
            Assert.IsFalse(result.BlobList.Any(x => x.URI.Contains("/Temp/SubFolderFile")));
            Assert.IsFalse(result.BlobList.Any(x => x.URI.Contains("/Temp/SubFolderFile2")));
            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/"));
            Assert.IsFalse(result.BlobList.Any(x => x.URI.Contains("/Temp/")));

            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile2.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.ETag != null));
            Assert.IsTrue(result.BlobList.Any(x => x.Type != null));
        }
    }


    private async Task CreateContainerAndTestFiles(bool delete)
    {
        var blobServiceClient = new BlobServiceClient(_connstring);
        var container = blobServiceClient.GetBlobContainerClient(_containerName);
        if (delete)
            await container.DeleteIfExistsAsync();
        else
        {
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());

            byte[] bytes;

            try
            {
                var files = new List<string>()
                {
                    "TestFile.txt", "TestFile2.txt", "Temp/SubFolderFile", "Temp/SubFolderFile2"
                };


                foreach(var file in files )
                {
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "Temp"));

                    var tempFile = Directory.GetCurrentDirectory() + "/" + file;
                    using (StreamWriter sw = File.CreateText(tempFile))
                        sw.WriteLine($"This is {file}");
                    
                    using (var reader = new StreamReader(tempFile)) 
                        bytes = Encoding.UTF32.GetBytes(reader.ReadToEnd());
                    
                    await container.UploadBlobAsync(file, new MemoryStream(bytes));
                    
                    if(File.Exists(tempFile))
                        File.Delete(tempFile);

                    if (Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Temp")))
                        Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Temp"), true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}