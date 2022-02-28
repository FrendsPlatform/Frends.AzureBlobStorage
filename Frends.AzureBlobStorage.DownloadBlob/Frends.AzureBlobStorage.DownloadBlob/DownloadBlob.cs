using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using Azure.Storage.Blobs;

#pragma warning disable CS1591
#pragma warning disable CS1573

namespace Frends.AzureBlobStorage.DownloadBlob
{
    public class AzureBlobStorage
    {
        /// <summary>
        /// Downloads Blob to a file.
        /// [Documentation](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/tree/main/Frends.AzureBlobStorage.DownloadBlob)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns>Object { string FileName, string Directory, string FullPath}</returns>
        public static async Task<Result> DownloadBlob(
            [PropertyTab] Input source,
            [PropertyTab] Options destination,
            CancellationToken cancellationToken)
        {
            var blob = new BlobClient(source.ConnectionString, source.ContainerName, source.BlobName);
            var fullDestinationPath = Path.Combine(destination.Directory, source.BlobName);
            var fileName = source.BlobName.Split('.')[0];
            var fileExtension = "";

            if (source.BlobName.Split('.').Length > 1)
            {
                fileName = string.Join(".", source.BlobName.Split('.').Take(source.BlobName.Split('.').Length - 1).ToArray());
                fileExtension = "." + source.BlobName.Split('.').Last();
            }

            if (destination.FileExistsOperation == FileExistsAction.Error && File.Exists(fullDestinationPath))
                throw new IOException("File already exists in destination path. Please delete the existing file or change the \"file exists operation\" to OverWrite.");

            if (destination.FileExistsOperation == FileExistsAction.Rename && File.Exists(fullDestinationPath))
            {
                var increment = 1;
                var incrementedFileName = fileName + "(" + increment.ToString() + ")" + fileExtension;

                while (File.Exists(Path.Combine(destination.Directory, incrementedFileName)))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    increment++;
                    incrementedFileName = fileName + "(" + increment.ToString() + ")" + fileExtension;
                }

                fullDestinationPath = Path.Combine(destination.Directory, incrementedFileName);
                fileName = incrementedFileName;
                await blob.DownloadToAsync(fullDestinationPath, cancellationToken);
            }
            else
            {
                await blob.DownloadToAsync(fullDestinationPath, cancellationToken);
            }

            CheckAndFixFileEncoding(fullDestinationPath, destination.Directory, fileExtension, source.Encoding);

            return new Result
            {
                Directory = destination.Directory,
                FileName = fileName,
                FullPath = fullDestinationPath
            };
        }

        #region HelperMethods

        /// <summary>
        ///     Check if the file encoding matches with given encoding and fix the encoding if it doesn't match.
        /// </summary>
        private static void CheckAndFixFileEncoding(string fullPath, string directory, string fileExtension, string targetEncoding)
        {
            var encoding = "";

            using (var reader = new StreamReader(fullPath, true))
            {
                reader.Read();
                encoding = reader.CurrentEncoding.BodyName;
            }

            if (targetEncoding.ToLower() != encoding)
            {
                Encoding newEncoding;

                try
                {
                    newEncoding = Encoding.GetEncoding(targetEncoding.ToLower());
                }
                catch
                {
                    throw new Exception("Provided encoding is not supported. Please check supported encodings from Encoding-option.");
                }

                var tempFilePath = Path.Combine(directory, "encodingTemp" + fileExtension);

                using (var sr = new StreamReader(fullPath, true))
                using (var sw = new StreamWriter(tempFilePath, false, newEncoding))
                {
                    var line = "";

                    while ((line = sr.ReadLine()) != null)
                    {
                        sw.WriteLine(line);
                    }
                }

                File.Delete(fullPath);
                File.Copy(tempFilePath, fullPath);
                File.Delete(tempFilePath);
            }
        }
        #endregion
    }
}
