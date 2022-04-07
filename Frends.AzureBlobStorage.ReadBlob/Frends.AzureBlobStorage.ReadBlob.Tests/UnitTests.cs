using NUnit.Framework;
using System;
using System.IO;

namespace Frends.AzureBlobStorage.ReadBlob
{
    [TestFixture]
    public class ReadTest
    {
        private static Source _source, _sourcefail;


        [OneTimeSetUp]
        public static void Setup()
        {
            var containername = "blobreadtest";
            var blobName = "blobtest.txt";
            var sastoken = "";
            var uri = $"https://__.blob.core.windows.net/{containername}/{blobName}?";
            var localConnectionString = "UseDevelopmentStorage=true";

            _source = new Source
            {
                //ConnectionString = localConnectionString,
                Uri = uri,
                SasToken = sastoken,
                ContainerName = containername,
                BlobName = blobName,
                Encoding = Encode.UTF8
            };

            _sourcefail = new Source
            {
                ConnectionString = localConnectionString,
                ContainerName = "nocontainer",
                BlobName = "nofile.txt",
                Encoding = Encode.UTF8
            };
        }

        [Test]
        public void ReadBlob()
        {
            var result = AzureBlobStorage.ReadBlob(_source, default);
        }

        [Test]
        public void ReadBlobBadConnection()
        {
            var result = AzureBlobStorage.ReadBlob(_sourcefail, default);
        }
    }
}
