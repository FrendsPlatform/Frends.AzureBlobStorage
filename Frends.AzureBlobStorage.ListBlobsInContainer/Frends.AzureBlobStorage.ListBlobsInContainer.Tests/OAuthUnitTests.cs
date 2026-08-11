using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using Frends.AzureBlobStorage.ListBlobsInContainer.Tests.lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

[TestClass]
public class OAuthUnitTests
{
    private readonly string _containerName =
        $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";

    [TestInitialize]
    public async Task Init()
    {
        TestHelper.LoadEnvironmentVariables();
        await Helper.CreateContainerAndTestFiles(false, TestHelper.ConnectionString, _containerName);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        await Helper.CreateContainerAndTestFiles(true, TestHelper.ConnectionString, _containerName);
    }

    [TestMethod]
    public async Task ListBlob_OAuth_ListingStructures()
    {
        var listing = new List<ListingStructure>()
        {
            ListingStructure.Flat,
            ListingStructure.Hierarchical
        };

        var input = new Input
        {
            ContainerName = _containerName,
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            ApplicationId = TestHelper.ClientId,
            TenantId = TestHelper.TenantId,
            ClientSecret = TestHelper.ClientSecret,
            StorageAccountName = TestHelper.StorageAccountName,
        };

        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = null,
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(input, connection, options, default);

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
    public async Task ListBlob_OAuth_Prefix()
    {
        var listing = new List<ListingStructure>()
        {
            ListingStructure.Flat,
            ListingStructure.Hierarchical
        };

        var input = new Input
        {
            ContainerName = _containerName,
        };

        var connection = new Connection
        {
            AuthenticationMethod = ConnectionMethod.OAuth2,
            ApplicationId = TestHelper.ClientId,
            TenantId = TestHelper.TenantId,
            ClientSecret = TestHelper.ClientSecret,
            StorageAccountName = TestHelper.StorageAccountName,
        };

        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = "Tes",
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(input, connection, options, default);

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
}
