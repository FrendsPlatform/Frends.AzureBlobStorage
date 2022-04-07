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
            var containername = "test";
            var blobName = "test.txt";
            var sastoken = "sp=r&st=2022-04-06T10:17:16Z&se=2022-04-11T18:17:16Z&spr=https&sv=2020-08-04&sr=b&sig=aG5N%2BywcDxb01QF9VZAUiEwTniUawJHZd4OsMFf21ow%3D";
            var uri = $"https://testsorage01.blob.core.windows.net/{containername}/{blobName}?";
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
