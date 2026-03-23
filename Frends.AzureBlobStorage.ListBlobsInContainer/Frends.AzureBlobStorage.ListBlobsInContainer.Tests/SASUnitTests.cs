using Azure.Storage;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Frends.AzureBlobStorage.ListBlobsInContainer.Tests.lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Frends.AzureBlobStorage.Toolkit.Definitions;
using Frends.AzureBlobStorage.Toolkit.Handlers;


namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

[TestClass]
public class SASUnitTests
{
    private readonly string _containerName =
        $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

    [TestInitialize]
    public async Task Init()
    {
        TestHandler.LoadEnvironmentVariables();
        await Helper.CreateContainerAndTestFiles(false, TestHandler.ConnectionString, _containerName);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        await Helper.CreateContainerAndTestFiles(true, TestHandler.ConnectionString, _containerName);
    }

    [TestMethod]
    public async Task ListBlob_SAS_MissingToken()
    {
        var source = new Source
        {
            ContainerName = _containerName,
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.SasToken,
            SasToken = string.Empty,
            StorageAccountName = TestHandler.StorageAccountName,
        };

        var options = new Options
        {
            Prefix = null,
            ListingStructure = ListingStructure.Flat,
        };

        var ex = await Assert.ThrowsExceptionAsync<ValidationException>(async () =>
            await AzureBlobStorage.ListBlobsInContainer(source, connection, options, default));
        Assert.IsTrue(ex.Message.Contains("SasToken is required."), ex.Message);

    }

    [TestMethod]
    public async Task ListBlobUnitTest_SAS_ListingStructures()
    {
        var listing = new List<ListingStructure>()
        {
            ListingStructure.Flat,
            ListingStructure.Hierarchical
        };

        var source = new Source
        {
            ContainerName = _containerName
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.SasToken,
            SasToken = GenerateSASToken(),
            StorageAccountName = TestHandler.StorageAccountName,
        };

        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = null,
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(source, connection, options, default);

            if (structure is ListingStructure.Flat)
            {
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile"));
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile2"));
                Assert.IsTrue(result.BlobList.Any(x => x.URL.Contains("/Temp/SubFolderFile")));
                Assert.IsTrue(result.BlobList.Any(x => x.URL.Contains("/Temp/SubFolderFile2")));
            }
            else
            {
                Assert.IsTrue(result.BlobList.Any(x => x.Name == "Temp/"));
                Assert.IsTrue(result.BlobList.Any(x => x.URL.Contains("/Temp/")));
            }

            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile2.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.ETag != null));
            Assert.IsTrue(result.BlobList.Any(x => x.Type != null));
            Assert.IsTrue(result.BlobList.Any(x => x.CreatedOn != null));
            Assert.IsTrue(result.BlobList.Any(x => x.LastModified != null));
        }
    }

    [TestMethod]
    public async Task ListBlob_SAS_Prefix()
    {
        var listing = new List<ListingStructure>()
        {
            ListingStructure.Flat,
            ListingStructure.Hierarchical
        };

        var source = new Source
        {
            ContainerName = _containerName,
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.SasToken,
            SasToken = GenerateSASToken(),
            StorageAccountName = TestHandler.StorageAccountName,
        };


        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = "Tes",
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(source, connection, options, default);

            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile"));
            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/SubFolderFile2"));
            Assert.IsFalse(result.BlobList.Any(x => x.URL.Contains("/Temp/SubFolderFile")));
            Assert.IsFalse(result.BlobList.Any(x => x.URL.Contains("/Temp/SubFolderFile2")));
            Assert.IsFalse(result.BlobList.Any(x => x.Name == "Temp/"));
            Assert.IsFalse(result.BlobList.Any(x => x.URL.Contains("/Temp/")));

            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.Name == "TestFile2.txt"));
            Assert.IsTrue(result.BlobList.Any(x => x.ETag != null));
            Assert.IsTrue(result.BlobList.Any(x => x.Type != null));
            Assert.IsTrue(result.BlobList.Any(x => x.CreatedOn != null));
            Assert.IsTrue(result.BlobList.Any(x => x.LastModified != null));
        }
    }

    private string GenerateSASToken()
    {
        BlobSasBuilder blobSasBuilder = new()
        {
            BlobContainerName = _containerName,
            ExpiresOn = DateTime.UtcNow.AddMinutes(5)
        };

        blobSasBuilder.SetPermissions(BlobContainerSasPermissions.List);
        var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(TestHandler.StorageAccountName, TestHandler.AccessKey))
            .ToString();

        return sasToken;
    }
}
