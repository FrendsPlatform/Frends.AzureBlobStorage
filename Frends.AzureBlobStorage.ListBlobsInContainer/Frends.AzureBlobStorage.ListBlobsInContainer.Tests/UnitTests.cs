using Azure.Storage;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;
using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Assert;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests
{
    [TestFixture]
    public class ListListBlobsInContainerTest
    {
        /// <summary>
        ///     Storage account's access key.
        /// </summary>
        private readonly string _accessKey = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_testsorage01AccessKey");

        /// <summary>
        ///     Storage account's connection string.
        /// </summary>
        private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        Source source;
        private readonly string _storageaccount = "testsorage01";
        private readonly string _containerName = "test";

        #region SAS Token

        /// <summary>
        ///     Test with SAS Token.
        /// </summary>
        [Test]
        public void ListBlobsSAS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://{_storageaccount}.blob.core.windows.net/",
                SasToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                Prefix = null,
                FlatBlobListing = true,
                
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with SAS Token and Prefix.
        /// </summary>
        [Test]
        public void ListBlobsSASPrefix()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://{_storageaccount}.blob.core.windows.net/",
                SasToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                Prefix = "t",
                FlatBlobListing = true,

            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with SAS Token and FlatBlobListing false.
        /// </summary>
        [Test]
        public void ListBlobsSASFBLf()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://{_storageaccount}.blob.core.windows.net/",
                SasToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                Prefix = null,
                FlatBlobListing = false,

            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with missing SAS Token.
        /// </summary>
        [Test]
        public void ListBlobsSASMissingToken()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://{_storageaccount}.blob.core.windows.net/",
                SasToken = "",
                ContainerName = _containerName,
                Prefix = null,
                FlatBlobListing = true,

            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await AzureBlobStorage.ListBlobsInContainer(source));
            Assert.That(ex.Message.Equals("SAS Token and URI required."));
        }
        #endregion SAS

        #region Connection string

        /// <summary>
        ///     Test with Connection string.
        /// </summary>
        [Test]
        public void ListBlobsCS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = _connstring,
                ContainerName = _containerName,
                Prefix = null,
                FlatBlobListing = true,
            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with connection string and Prefix.
        /// </summary>
        [Test]
        public void ListBlobsCSPrefix()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = _connstring,
                ContainerName = _containerName,
                Prefix = "t",
                FlatBlobListing = true,

            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with connection string and FlatBlobListing false.
        /// </summary>
        [Test]
        public void ListBlobsCSFBLf()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = _connstring,
                ContainerName = _containerName,
                Prefix = null,
                FlatBlobListing = false,

            };

            var result = AzureBlobStorage.ListBlobsInContainer(source);
            Assert.IsNotEmpty(result.Result.BlobList);
        }

        /// <summary>
        ///     Test with missing connection string.
        /// </summary>
        [Test]
        public void ListBlobsMissingCS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = "",
                ContainerName = _containerName,
                Prefix = "t",
                FlatBlobListing = false,

            };

            var ex = Assert.ThrowsAsync<Exception>(async () => await AzureBlobStorage.ListBlobsInContainer(source));
            Assert.That(ex.Message.Equals("Connection string required."));
        }
        #endregion SAS


        /// <summary>
        ///     Generate SAS Token. Token last for 5 minutes.
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
