using System;
using System.Threading;
using NUnit.Framework;
using Frends.AzureBlobStorage.WriteBlob.Definitions;
using Frends.AzureBlobStorage.WriteBlob.Enums;

namespace Frends.AzureBlobStorage.WriteBlob.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input DefaultInput() => new Input
    {
        SourceType = SourceType.String,
        ContentString = "test",
        ContainerName = "testcontainer",
        BlobName = "testblob.txt",
        HandleExistingFile = HandleExistingFile.Overwrite,
        Encoding = FileEncoding.UTF8,
    };

    private static Connection DefaultConnection() => new Connection
    {
        AuthenticationMethod = ConnectionMethod.ConnectionString,
        ConnectionString = "DefaultEndpointsProtocol=https;AccountName=invalid;AccountKey=InvalidAccountKey==;EndpointSuffix=core.windows.net",
    };

    private static Options DefaultOptions() => new Options
    {
        ThrowErrorOnFailure = true,
        ErrorMessageOnFailure = string.Empty,
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await AzureBlobStorage.WriteBlob(DefaultInput(), DefaultConnection(), DefaultOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async System.Threading.Tasks.Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = await AzureBlobStorage.WriteBlob(DefaultInput(), DefaultConnection(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.ThrowsAsync<Exception>(async () =>
            await AzureBlobStorage.WriteBlob(DefaultInput(), DefaultConnection(), options, CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }
}
