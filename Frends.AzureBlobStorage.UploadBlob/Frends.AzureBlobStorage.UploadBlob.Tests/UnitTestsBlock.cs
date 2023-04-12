using Azure.Storage.Blobs;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

[TestClass]
public class BlockTests
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ConnString");
    private string _containerName;
    private readonly string _appID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_AppID");
    private readonly string _clientSecret = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_ClientSecret");
    private readonly string _tenantID = Environment.GetEnvironmentVariable("HiQ_AzureBlobStorage_TenantID");
    private readonly string _storageAccount = "testsorage01";
    private readonly string _testFileDir = Path.Combine(Environment.CurrentDirectory, "TestFiles");
    private readonly string _testfile = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile.txt");
    private readonly string _testfile2 = Path.Combine(Environment.CurrentDirectory, "TestFiles", "testfile2.txt");
    private readonly string _testfile3 = Path.Combine(Environment.CurrentDirectory, "TestFiles", "SearchPattern.xml");

    [TestInitialize]
    public void TestSetup()
    {
        CreateFiles();
        _containerName = $"test-container{DateTime.Now.ToString("mmssffffff", CultureInfo.InvariantCulture)}";
    }

    [TestCleanup]
    public async Task CleanUp()
    {
        var container = GetBlobContainer(_connectionString, _containerName);
        await container.DeleteIfExistsAsync();
        DeleteFiles();
    }

    [TestMethod]
    public async Task UploadFile_WithAndWithoutTags()
    {
        var tags = new Tag[] { new Tag { Name = "TagName", Value = "TagValue" } };
        var options = new Options() { ThrowErrorOnFailure = true };

        var inputWithTags = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = tags,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = default,
            BlobFolderName = default,
        };

        var inputWithoutTags = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = default,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = default,
            BlobFolderName = default,
        };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = default,
            ApplicationID = default,
            StorageAccountName = default,
            ClientSecret = default,
            ContentType = default,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = default,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = default,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var container = GetBlobContainer(_connectionString, _containerName);

        // connection string
        var resultWithTags = await AzureBlobStorage.UploadBlob(inputWithTags, destinationCS, options, default);
        Assert.IsTrue(resultWithTags.Success);
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

        var resultWithoutTags = await AzureBlobStorage.UploadBlob(inputWithoutTags, destinationCS, options, default);
        Assert.IsTrue(resultWithoutTags.Success);
        Assert.IsTrue(resultWithoutTags.Data.ContainsValue($"{container.Uri}/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

        // OAuth
        var resultWithTags2 = await AzureBlobStorage.UploadBlob(inputWithTags, destinationOA, options, default);
        Assert.IsTrue(resultWithTags2.Success);
        Assert.IsTrue(resultWithTags2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

        var resultWithoutTags2 = await AzureBlobStorage.UploadBlob(inputWithoutTags, destinationOA, options, default);
        Assert.IsTrue(resultWithoutTags2.Success);
        Assert.IsTrue(resultWithoutTags2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
    }

    [TestMethod]
    public async Task UploadFile_SetBlobName_ForceEncoding_ForceContentType()
    {
        var options = new Options() { ThrowErrorOnFailure = true };

        var input = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = default,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = "SomeBlob",
            BlobFolderName = default,
        };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "foo/bar",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = default,
            ApplicationID = default,
            StorageAccountName = default,
            ClientSecret = default,
            ContentType = "text/xml",
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "foo/bar",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = "text/xml",
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var container = GetBlobContainer(_connectionString, _containerName);

        // Connection string
        var result = await AzureBlobStorage.UploadBlob(input, destinationCS, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
        Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

        //OAuth
        var result2 = await AzureBlobStorage.UploadBlob(input, destinationOA, options, default);
        Assert.IsTrue(result2.Success);
        Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/SomeBlob"));
        Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");
    }

    [TestMethod]
    public async Task UploadFile_UpperCaseContainerNameFix()
    {
        var options = new Options() { ThrowErrorOnFailure = true };

        var input = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = default,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = "SomeBlob",
            BlobFolderName = default,
        };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName.ToUpper(),
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "foo/bar",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = default,
            ApplicationID = default,
            StorageAccountName = default,
            ClientSecret = default,
            ContentType = "text/xml",
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName.ToUpper(),
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "foo/bar",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = "text/xml",
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var container = GetBlobContainer(_connectionString, _containerName);

        // Connection string
        var result = await AzureBlobStorage.UploadBlob(input, destinationCS, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/SomeBlob"));
        Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");

        //OAuth
        var result2 = await AzureBlobStorage.UploadBlob(input, destinationOA, options, default);
        Assert.IsTrue(result2.Success);
        Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/SomeBlob"));
        Assert.IsTrue(await container.GetBlobClient("SomeBlob").ExistsAsync(), "Uploaded SomeBlob blob should exist");
    }

    [TestMethod]
    public async Task UploadFile_Compress()
    {
        var options = new Options() { ThrowErrorOnFailure = true };

        var input = new Source
        {
            SourceFile = _testfile,
            Tags = default,
            Compress = true,
            ContentsOnly = true,
            SourceType = UploadSourceType.File,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = "compress.gz",
            BlobFolderName = default,
        };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = null,
            ApplicationID = null,
            StorageAccountName = null,
            ClientSecret = null,
            ContentType = null,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = null,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };


        var container = GetBlobContainer(_connectionString, _containerName);

        // connection string
        var result = await AzureBlobStorage.UploadBlob(input, destinationCS, options, default);
        Assert.IsTrue(result.Success);
        Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/compress.gz"));
        Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");

        // OAuth
        var result2 = await AzureBlobStorage.UploadBlob(input, destinationOA, options, default);
        Assert.IsTrue(result2.Success);
        Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/compress.gz"));
        Assert.IsTrue(await container.GetBlobClient("compress.gz").ExistsAsync(), "Uploaded SomeBlob blob should exist");
    }

    [TestMethod]
    public async Task UploadFile_HandleExistingFile()
    {
        var errorHandlers = new List<HandleExistingFile>() { HandleExistingFile.Append, HandleExistingFile.Overwrite, HandleExistingFile.Error };
        var options = new Options() { ThrowErrorOnFailure = true };

        var input = new Source
        {
            SourceType = UploadSourceType.File,
            SourceFile = _testfile,
            Tags = default,
            Compress = default,
            ContentsOnly = default,
            SearchPattern = default,
            SourceDirectory = default,
            BlobName = default,
            BlobFolderName = default,
        };

        foreach (var handler in errorHandlers)
        {
            var destination = new Destination
            {
                ConnectionMethod = ConnectionMethod.ConnectionString,
                ContainerName = _containerName,
                BlobType = AzureBlobType.Block,
                ConnectionString = _connectionString,
                CreateContainerIfItDoesNotExist = true,
                FileEncoding = "utf-8",
                HandleExistingFile = handler,
                TenantID = default,
                ApplicationID = default,
                StorageAccountName = default,
                ClientSecret = default,
                ContentType = default,
                PageMaxSize = default,
                PageOffset = default,
                ParallelOperations = default,
            };

            var container = GetBlobContainer(_connectionString, _containerName);
            var result = await AzureBlobStorage.UploadBlob(input, destination, options, default);
            Assert.IsTrue(result.Success);
            Assert.IsTrue(result.Data.ContainsValue($"{container.Uri}/testfile.txt"));
            Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");

            if (handler is HandleExistingFile.Append)
            {
                var result2 = await AzureBlobStorage.UploadBlob(input, destination, options, default);
                // Ýou can use Azure Portal to check if the blob contains 2x "Etiam dui".
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            }
            else if (handler is HandleExistingFile.Overwrite)
            {
                var result2 = await AzureBlobStorage.UploadBlob(input, destination, options, default);
                Assert.IsTrue(result2.Success);
                Assert.IsTrue(result2.Data.ContainsValue($"{container.Uri}/testfile.txt"));
                Assert.IsTrue(await container.GetBlobClient("testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            }
            else
            {
                var ex = await Assert.ThrowsExceptionAsync<Exception>(() => AzureBlobStorage.UploadBlob(input, destination, options, default));
                Assert.IsTrue(ex.InnerException.InnerException.Message.Contains("already exists"));
            }

            await CleanUp();
            TestSetup();
        }
    }

    [TestMethod]
    public async Task UploadDirectory_WithAndWithoutTags_TestXLSXFile()
    {
        CreateXlsxFile();

        var tags = new Tag[] { new Tag { Name = "TagName", Value = "TagValue" } };
        var options = new Options() { ThrowErrorOnFailure = true };

        var inputWithTags = new Source
        {
            SourceType = UploadSourceType.Directory,
            SourceDirectory = _testFileDir,
            SearchPattern = default,
            ContentsOnly = default,
            Tags = tags,
            Compress = default,
            SourceFile = default,
            BlobName = default,
            BlobFolderName = default,
        };

        var inputWithoutTags = new Source
        {
            SourceType = UploadSourceType.Directory,
            SourceDirectory = _testFileDir,
            SearchPattern = default,
            ContentsOnly = default,
            Tags = default,
            Compress = default,
            SourceFile = default,
            BlobName = default,
            BlobFolderName = default,
        };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = default,
            ApplicationID = default,
            StorageAccountName = default,
            ClientSecret = default,
            ContentType = default,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var destinationOA = new Destination
        {
            ConnectionMethod = ConnectionMethod.OAuth2,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = default,
            CreateContainerIfItDoesNotExist = false,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = _tenantID,
            ApplicationID = _appID,
            StorageAccountName = _storageAccount,
            ClientSecret = _clientSecret,
            ContentType = default,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var container = GetBlobContainer(_connectionString, _containerName);

        // connection string
        var resultWithTags = await AzureBlobStorage.UploadBlob(inputWithTags, destinationCS, options, default);
        Assert.IsTrue(resultWithTags.Success);
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/test.xlsx"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/test.xlsx").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

        var resultWithoutTags = await AzureBlobStorage.UploadBlob(inputWithoutTags, destinationCS, options, default);
        Assert.IsTrue(resultWithoutTags.Success);
        Assert.IsTrue(resultWithoutTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        Assert.IsTrue(resultWithoutTags.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/test.xlsx"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/test.xlsx").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

        // OAuth
        var resultWithTags2 = await AzureBlobStorage.UploadBlob(inputWithTags, destinationOA, options, default);
        Assert.IsTrue(resultWithTags2.Success);
        Assert.IsTrue(resultWithTags2.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        Assert.IsTrue(resultWithTags2.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/test.xlsx"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/test.xlsx").ExistsAsync(), "Uploaded testfile2.txt blob should exist");

        var resultWithoutTags2 = await AzureBlobStorage.UploadBlob(inputWithoutTags, destinationOA, options, default);
        Assert.IsTrue(resultWithoutTags2.Success);
        Assert.IsTrue(resultWithoutTags2.Data.ContainsValue($"{container.Uri}/TestFiles/testfile.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile.txt").ExistsAsync(), "Uploaded testfile.txt blob should exist");
        Assert.IsTrue(resultWithoutTags2.Data.ContainsValue($"{container.Uri}/TestFiles/testfile2.txt"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/testfile2.txt").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
        Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/test.xlsx"));
        Assert.IsTrue(await container.GetBlobClient("TestFiles/test.xlsx").ExistsAsync(), "Uploaded testfile2.txt blob should exist");
    }

    [TestMethod]
    public async Task UploadDirectory_SearchPatterns()
    {
        var searchPatterns = new[] { "Search*.*", "*.xml" };
        var options = new Options() { ThrowErrorOnFailure = true };

        var destinationCS = new Destination
        {
            ConnectionMethod = ConnectionMethod.ConnectionString,
            ContainerName = _containerName,
            BlobType = AzureBlobType.Block,
            ConnectionString = _connectionString,
            CreateContainerIfItDoesNotExist = true,
            FileEncoding = "utf-8",
            HandleExistingFile = HandleExistingFile.Overwrite,
            TenantID = default,
            ApplicationID = default,
            StorageAccountName = default,
            ClientSecret = default,
            ContentType = default,
            PageMaxSize = default,
            PageOffset = default,
            ParallelOperations = default,
        };

        var container = GetBlobContainer(_connectionString, _containerName);

        foreach (var sp in searchPatterns)
        {
            var input = new Source
            {
                SourceType = UploadSourceType.Directory,
                SourceDirectory = _testFileDir,
                SearchPattern = sp,
                ContentsOnly = default,
                Tags = default,
                Compress = default,
                SourceFile = default,
                BlobName = default,
                BlobFolderName = default,
            };

            // connection string
            var resultWithTags = await AzureBlobStorage.UploadBlob(input, destinationCS, options, default);
            Assert.IsTrue(resultWithTags.Success);
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/SearchPattern.xml"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/SearchPattern.xml").ExistsAsync(), "Uploaded testfile.txt blob should exist");
            Assert.IsTrue(resultWithTags.Data.ContainsValue($"{container.Uri}/TestFiles/SearchPattern.xml"));
            Assert.IsTrue(await container.GetBlobClient("TestFiles/SearchPattern.xml").ExistsAsync(), "Uploaded SearchPattern.xml blob should exist");
        }
    }

    private static BlobContainerClient GetBlobContainer(string connectionString, string containerName)
    {
        var blobServiceClient = new BlobServiceClient(connectionString);
        return blobServiceClient.GetBlobContainerClient(containerName);
    }

    private void CreateFiles()
    {
        Directory.CreateDirectory(_testFileDir);

        // 512 byte file
        File.WriteAllText(_testfile, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non sem quis orci rutrum hendrerit. Fusce ultricies cursus ante nec bibendum. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam libero massa, viverra id suscipit in, tincidunt sit amet urna. Vestibulum gravida a massa eu molestie. Phasellus volutpat neque vitae enim molestie, vitae pharetra massa varius. Phasellus ante nulla, faucibus nec tristique eu, dignissim quis magna. Sed vitae sodales ipsum. Ut et maximus nibh. Etiam dui.");
        File.WriteAllText(_testfile2, "More texts.");
        File.WriteAllText(_testfile3, "Even more texts.");
    }

    private void CreateXlsxFile()
    {
        using var document = SpreadsheetDocument.Create(Path.Combine(_testFileDir, "test.xlsx"), SpreadsheetDocumentType.Workbook);
        var workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        var sheets = workbookPart.Workbook.AppendChild(new Sheets());
        var sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };
        sheets.Append(sheet);

        var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
        for (int i = 1; i <= 5; i++)
        {
            var row = new Row() { RowIndex = (uint)i };
            var cell1 = new Cell() { CellReference = "A" + i, DataType = CellValues.String, CellValue = new CellValue("Data " + i.ToString()) };
            var cell2 = new Cell() { CellReference = "B" + i, DataType = CellValues.Number, CellValue = new CellValue((i * 100).ToString()) };
            row.Append(cell1, cell2);
            sheetData.Append(row);
        }

        workbookPart.Workbook.Save();
    }

    private void DeleteFiles()
    {
        if (Directory.Exists(_testFileDir))
            Directory.Delete(_testFileDir, true);
    }
}