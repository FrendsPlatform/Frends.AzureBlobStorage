using Azure.Storage;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using NUnit.Framework;
using System;
using Assert = NUnit.Framework.Assert;

namespace Frends.AzureBlobStorage.ReadBlob.Tests
{
    [TestFixture]
    public class ReadTest
    {
        /// <summary>
        ///     Add Storage account's access key to Environment Variables
        /// </summary>
        private readonly string _accessKey = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_testsorage01AccessKey");

        /// <summary>
        ///     Add connection string to Environment Variables
        /// </summary>
        private readonly string _connstring = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");

        Source source;
        private readonly string _storageaccount = "testsorage01";
        private readonly string _containerName = "test";
        private readonly string _blobName = "test.txt";

        /// <summary>
        ///     Test with SAS Token
        /// </summary>
        [Test]
        public void ReadBlobSAS()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://testsorage01.blob.core.windows.net/{_containerName}/{_blobName}?",
                SasToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                BlobName = _blobName,
                Encoding = Encode.ASCII
            };

            var result = AzureBlobStorage.ReadBlob(source, default);
            Assert.IsNotEmpty(result.Content);
        }

        /// <summary>
        ///     Test with Connection string
        /// </summary>
        [Test]
        public void ReadBlobConnectionString()
        {
            source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = _connstring,
                ContainerName = _containerName,
                BlobName = _blobName,
                Encoding = Encode.ASCII
            };

            var result = AzureBlobStorage.ReadBlob(source, default);
            Assert.IsNotEmpty(result.Content);
        }

        /// <summary>
        ///     Error handling, missing SAS Token error
        /// </summary>
        [Test]
        public void ReadBlobSasMissing()
        {
            var source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                Uri = $"https://testsorage01.blob.core.windows.net/{_containerName}/{_blobName}?",
                SasToken = "",
                ContainerName = _containerName,
                BlobName = _blobName,
                Encoding = Encode.ASCII
            };

            var ex = Assert.Throws<Exception>(() => AzureBlobStorage.ReadBlob(source, default));
            Assert.That(ex.Message.Equals("SAS Token and URI required."));
        }

        /// <summary>
        ///     Error handling, missing connection string
        /// </summary>
        [Test]
        public void ReadBlobConnectionStringMissing()
        {
            var source = new Source
            {
                AuthenticationMethod = AuthenticationMethod.Connectionstring,
                ConnectionString = "",
                ContainerName = _containerName,
                BlobName = _blobName,
                Encoding = Encode.ASCII
            };

            var ex = Assert.Throws<Exception>(() => AzureBlobStorage.ReadBlob(source, default));
            Assert.That(ex.Message.Equals("Connection string required."));
        }


        /// <summary>
        ///     Generate SAS Token for testfile. Token last for 10 minutes.
        /// </summary>
        private string GetServiceSasUriForBlob()
        {
            BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = _containerName,
                BlobName = _blobName,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5)
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_storageaccount, _accessKey)).ToString();
            return sasToken;
        }
    }

}
