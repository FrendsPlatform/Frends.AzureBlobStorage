using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Frends.AzureBlobStorage.ReadBlob.Definitions;
using NUnit.Framework;
using System;
using System.IO;

namespace Frends.AzureBlobStorage.ReadBlob
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

        private Source _source, _sourcefail;
        private readonly string _storageaccount = "testsorage01";
        private readonly string _containerName = "test";
        private readonly string _blobName = "test.txt";


        [OneTimeSetUp]
        public void Setup()
        {
            _source = new Source
            {
                //AuthenticationMethod = AuthenticationMethod.Connectionstring,
                AuthenticationMethod = AuthenticationMethod.Sastoken,
                ConnectionString = _connstring, //Comment this line to test with SAS Token.
                Uri = $"https://testsorage01.blob.core.windows.net/{_containerName}/{_blobName}?",
                SasToken = GetServiceSasUriForBlob(),
                ContainerName = _containerName,
                BlobName = _blobName,
                Encoding = Encode.UTF8
            };

            _sourcefail = new Source
            {
                ConnectionString = _connstring,
                ContainerName = "nocontainer",
                BlobName = "nofile.txt",
                Encoding = Encode.UTF8
            };
        }

        /// <summary>
        ///     Test with SAS Token or connection string
        /// </summary>
        [Test]
        public void ReadBlob()
        {
            var result = AzureBlobStorage.ReadBlob(_source, default);
        }

        /// <summary>
        ///     Error handling
        /// </summary>
        [Test]
        public void ReadBlobBadConnection()
        {
            var result = AzureBlobStorage.ReadBlob(_sourcefail, default);
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
                ExpiresOn = DateTime.UtcNow.AddMinutes(10)
            };
            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
            var sasToken = blobSasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(_storageaccount, _accessKey)).ToString();
            return sasToken;
        }
    }

}
