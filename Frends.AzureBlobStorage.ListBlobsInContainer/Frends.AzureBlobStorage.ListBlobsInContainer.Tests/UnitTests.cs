using Azure.Storage;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using NUnit.Framework;
using System;
using System.Threading;
using Assert = NUnit.Framework.Assert;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests
{
    [TestFixture]
    public class ListListBlobsInContainerTest
    {
        /// <summary>
        /// Storage account's access key.
        /// </summary>
        private readonly string _accessKey = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_testsorage01AccessKey");

        /// <summary>
        /// Storage account's connection string.
        /// </summary>
        private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        Source source;
        Options options;
        private readonly string _storageaccount = "testsorage01";
        private readonly string _containerName = "test";

        #region SAS Token

        /// <summary>
        /// Test with SAS Token and Flat listing.
        /// </summary>
        [Test]
        public void ListBlobsSASFlat()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.SASToken,
                URI = $"https://{_storageaccount}.blob.core.windows.net",
                SASToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName
            };

            options = new Options
            {
                Prefix = null,
                ListingStructure = ListingStructure.Flat
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }


        /// <summary>
        /// Test with SAS Token and Hierarchical listing.
        /// </summary>
        [Test]
        public void ListBlobsSASHierarchical()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.SASToken,
                URI = $"https://{_storageaccount}.blob.core.windows.net",
                SASToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName
            };

            options = new Options
            {
                Prefix = null,
                ListingStructure = ListingStructure.Hierarchical,
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }


        /// <summary>
        /// Test with SAS Token and Prefix.
        /// </summary>
        [Test]
        public void ListBlobsSASPrefix()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.SASToken,
                URI = $"https://{_storageaccount}.blob.core.windows.net",
                SASToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                
            };

            options = new Options
            {
                Prefix = "t",
                ListingStructure = ListingStructure.Flat,
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        /// Test with missing SAS Token.
        /// </summary>
        [Test]
        public void ListBlobsSASMissingToken()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.SASToken,
                URI = $"https://{_storageaccount}.blob.core.windows.net",
                SASToken = "",
                ContainerName = _containerName,
                
            };

            options = new Options
            {
                Prefix = null,
                ListingStructure = ListingStructure.Flat,
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await AzureBlobStorage.ListBlobsInContainer(source, options, default));
            Assert.That(ex.Message.Equals("SAS Token and URI required."));
        }

        #endregion SAS

        #region ConnectionString

        /// <summary>
        /// Test with Connection string and Flat listing.
        /// </summary>
        [Test]
        public void ListBlobsCS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.ConnectionString,
                ConnectionString = _connstring,
                ContainerName = _containerName
            };

            options = new Options
            {
                Prefix = null,
                ListingStructure = ListingStructure.Flat,
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        /// Test with connection string and Prefix.
        /// </summary>
        [Test]
        public void ListBlobsCSPrefix()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.ConnectionString,
                ConnectionString = _connstring,
                ContainerName = _containerName
            };

            options = new Options
            {
                Prefix = "test",
                ListingStructure = ListingStructure.Flat
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        /// Test with connection string and FlatBlobListing false.
        /// </summary>
        [Test]
        public void ListBlobsCSHierarchical()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.ConnectionString,
                ConnectionString = _connstring,
                ContainerName = _containerName

            };

            options = new Options
            {
                Prefix = null,
                ListingStructure = ListingStructure.Hierarchical
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source, options, default);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        /// Test with missing connection string.
        /// </summary>
        [Test]
        public void ListBlobsMissingCS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.ConnectionString,
                ConnectionString = "",
                ContainerName = _containerName,
            };

            options = new Options
            {
                Prefix = "t",
                ListingStructure = ListingStructure.Hierarchical
            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await AzureBlobStorage.ListBlobsInContainer(source, options, default));
            Assert.That(ex.Message.Equals("Connection string required."));
        }

        #endregion ConnectionString


        /// <summary>
        /// Generate SAS Token. Token last for 5 minutes.
        /// </summary>
        private string GetServiceSasUriForBlob()
        {
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5)
            };
            //blobSasBuilder.SetPermissions(BlobAccountSasPermissions.List);
            blobSasBuilder.SetPermissions(BlobContainerSasPermissions.List);
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_storageaccount, _accessKey)).ToString();
            return sasToken;
        }
    }
}