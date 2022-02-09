using System.IO;
using System.IO.Compression;
using System.Text;
using Azure.Storage.Blobs;

#pragma warning disable CS1591

namespace Frends.AzureBlobStorage.UploadBlob
{
    public class Utils
    {
        public static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
        {
            // Initialize azure account.
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Fetch the container client.
            return blobServiceClient.GetBlobContainerClient(containerName);
        }

        /// <summary>
        ///     Gets correct stream object.
        ///     Does not always dispose, so use using.
        /// </summary>
        /// <param name="compress"></param>
        /// <param name="file"></param>
        /// <param name="fromString"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream GetStream(bool compress, bool fromString, Encoding encoding, FileInfo file)
        {
            var fileStream = File.OpenRead(file.FullName);

            if (!compress && !fromString) return fileStream; // As uncompressed binary.

            byte[] bytes;
            if (!compress)
            {
                using (var reader = new StreamReader(fileStream, encoding)) bytes = encoding.GetBytes(reader.ReadToEnd());
                return new MemoryStream(bytes); // As uncompressed string.
            }

            using (var outStream = new MemoryStream())
            {
                using (var gzip = new GZipStream(outStream, CompressionMode.Compress))
                {
                    if (!fromString) fileStream.CopyTo(gzip); // As compressed binary.
                    else
                        using (var reader = new StreamReader(fileStream, encoding))
                        {
                            var content = reader.ReadToEnd();
                            using (var encodedMemory = new MemoryStream(encoding.GetBytes(content))) encodedMemory.CopyTo(gzip); // As compressed string.
                        }
                }
                bytes = outStream.ToArray();
            }
            fileStream.Dispose();

            var memStream = new MemoryStream(bytes);
            return memStream;
        }
    }
}