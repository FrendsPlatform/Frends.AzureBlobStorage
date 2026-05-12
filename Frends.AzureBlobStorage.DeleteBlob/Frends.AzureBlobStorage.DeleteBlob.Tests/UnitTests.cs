using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Frends.AzureBlobStorage.DeleteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.AzureBlobStorage.DeleteBlob.Tests;

[TestClass]
public class DeleteTest
{
    private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
    private readonly string _firstTestFile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
    private readonly string _secondTestFile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");
    private string _containerName;

    [TestInitialize]
    public async Task TestSetup()
    {
        try
        {
            TestHelper.LoadEnvironmentVariables();
            _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
            var container = GetBlobContainer(TestHelper.ConnectionString, _containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.None, null, null, new CancellationToken());
            await CreateFiles();
            await UploadTestFiles(new FileInfo(_firstTestFile));
            await UploadTestFiles(new FileInfo(_secondTestFile));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        var container = GetBlobContainer(TestHelper.ConnectionString, _containerName);
        await container.DeleteIfExistsAsync();
        Directory.Delete(_testFileDir, true);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_ConnectionString_ContainerDoesNotExist_ReturnsFalse()
    {
        var input = new Input
        {
            BlobName = Guid.NewGuid().ToString(),
            ContainerName = "none",
        };

        var connection = new Connection()
        {
            ConnectionString = TestHelper.ConnectionString,
        };

        var options = new Options()
        {
            SnapshotDeleteOption = default,
            VerifyETagWhenDeleting = default,
            ThrowErrorOnFailure = false,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
        Assert.IsTrue(result.Error.Message.Contains("doesn't exist in container"));
    }

    [TestMethod]
    public async Task DeleteBlobAsync_FailOnBlobNotFound_True_ThrowsException()
    {
        var input = new Input
        {
            BlobName = Guid.NewGuid().ToString(),
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            ConnectionString = TestHelper.ConnectionString,
        };

        var options = new Options
        {
            SnapshotDeleteOption = default,
            VerifyETagWhenDeleting = default,
            ThrowErrorOnFailure = true,
        };

        var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
            AzureBlobStorage.DeleteBlob(input, connection, options, CancellationToken.None));

        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_OAuth_BlobDoesNotExist_ReturnsFalse()
    {
        var input = new Input
        {
            BlobName = "none",
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            ApplicationId = TestHelper.ClientId,
            StorageAccountName = TestHelper.StorageAccountName,
            ClientSecret = TestHelper.ClientSecret,
            AuthenticationMethod = ConnectionMethod.OAuth2,
            TenantId = TestHelper.TenantId,
        };

        var options = new Options()
        {
            SnapshotDeleteOption = default,
            VerifyETagWhenDeleting = default,
            ThrowErrorOnFailure = false,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_ConnectionString_DeleteBlob_ReturnsSuccess()
    {
        var input = new Input
        {
            BlobName = new FileInfo(_firstTestFile).Name,
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            AuthenticationMethod = ConnectionMethod.ConnectionString,
            ConnectionString = TestHelper.ConnectionString
        };

        var options = new Options()
        {
            SnapshotDeleteOption = default,
            VerifyETagWhenDeleting = default,
            ThrowErrorOnFailure = true,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_OAuth_DeleteBlob_ReturnsSuccess()
    {
        var input = new Input
        {
            BlobName = new FileInfo(_firstTestFile).Name,
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            ApplicationId = TestHelper.ClientId,
            StorageAccountName = TestHelper.StorageAccountName,
            ClientSecret = TestHelper.ClientSecret,
            AuthenticationMethod = ConnectionMethod.OAuth2,
            TenantId = TestHelper.TenantId,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, new Options(), default);
        Assert.IsTrue(result.Success);
        Assert.IsNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_ConnectionString_BlobNotFullName_ReturnsFalse()
    {
        var input = new Input
        {
            BlobName = "testfile",
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            ConnectionString = TestHelper.ConnectionString,
            AuthenticationMethod = ConnectionMethod.ConnectionString,
        };

        var options = new Options()
        {
            ThrowErrorOnFailure = false,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DeleteBlobAsync_ConnectionString_BlobWildcard_ReturnsFalse()
    {
        var input = new Input
        {
            BlobName = "testfile*",
            ContainerName = _containerName,
        };

        var connection = new Connection()
        {
            ConnectionString = TestHelper.ConnectionString,
            AuthenticationMethod = ConnectionMethod.ConnectionString,
        };

        var options = new Options()
        {
            ThrowErrorOnFailure = false,
        };

        var result = await AzureBlobStorage.DeleteBlob(input, connection, options, default);
        Assert.IsFalse(result.Success);
        Assert.IsNotNull(result.Error);
    }

    private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);

        return blobServiceClient.GetBlobContainerClient(containerName);
    }

    private async Task CreateFiles()
    {
        Directory.CreateDirectory(_testFileDir);

        #region 512 byte files

        await File.WriteAllTextAsync(_firstTestFile,
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");

        await File.WriteAllTextAsync(_secondTestFile,
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla mollis neque nibh, molestie consequat lacus euismod in. Phasellus in libero id velit sollicitudin rhoncus. Sed a nunc non lacus hendrerit iaculis. Suspendisse quis dui id enim sollicitudin rhoncus. Phasellus convallis lacinia finibus. Sed quis purus vitae felis finibus facilisis at quis nisi. Integer pharetra, ex egestas iaculis ultricies, tortor neque hendrerit justo, eu vulputate odio eros sed augue. Mauris quis sapien non ligula maximus eget.");

        #endregion 512 byte files
    }

    private async Task UploadTestFiles(FileInfo fileInfo)
    {
        try
        {
            using var fileStream = File.OpenRead(fileInfo.FullName);
            var blobClient = new BlobClient(TestHelper.ConnectionString, _containerName, fileInfo.Name);
            await blobClient.UploadAsync(fileStream);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
