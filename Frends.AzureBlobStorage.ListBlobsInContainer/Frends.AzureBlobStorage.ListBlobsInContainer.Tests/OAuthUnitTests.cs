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
    private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private readonly string _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
    private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
    private readonly string _uri = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_URI");

    [TestInitialize]
    public async Task Init()
    {
        await Helper.CreateContainerAndTestFiles(false, _connstring, _containerName);
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        await Helper.CreateContainerAndTestFiles(true, _connstring, _containerName);
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

        foreach (var structure in listing)
        {
            var options = new Options
            {
                Prefix = null,
                ListingStructure = structure
            };

            var result = await AzureBlobStorage.ListBlobsInContainer(source, options, default);

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