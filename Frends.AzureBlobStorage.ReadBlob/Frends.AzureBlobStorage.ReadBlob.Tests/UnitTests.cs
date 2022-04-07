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
            var sastoken = "sp=r&st=2022-04-07T06:27:17Z&se=2022-04-07T14:27:17Z&spr=https&sv=2020-08-04&sr=b&sig=Us%2B3HXz%2Fj%2BmvlRw8L9YcrR%2BF1b4ttB%2F8Fhi%2FYn4w9%2F8%3D";
            var uri = $"https://teemusbfrdstrg.blob.core.windows.net/{containername}/{blobName}?";
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
