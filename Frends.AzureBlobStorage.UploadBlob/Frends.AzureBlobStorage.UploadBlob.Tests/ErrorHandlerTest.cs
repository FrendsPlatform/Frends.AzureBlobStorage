using System;
using System.Threading;
using Frends.AzureBlobStorage.UploadBlob.Definitions;
using NUnit.Framework;

namespace Frends.AzureBlobStorage.UploadBlob.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input InvalidInput() => new Input
    {
        ContainerName = "test",
        SourceType = UploadSourceType.File,
        SourceFile = string.Empty,
    };

    private static Connection InvalidConnection() => new Connection
    {
        AuthenticationMethod = ConnectionMethod.ConnectionString,
        ConnectionString = string.Empty,
    };

    private static Options DefaultOptions() => new Options
    {
        ThrowErrorOnFailure = true,
    };

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Throws<Exception>(() =>
            AzureBlobStorage.UploadBlob(InvalidInput(), InvalidConnection(), DefaultOptions(), CancellationToken.None).GetAwaiter().GetResult());
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = AzureBlobStorage.UploadBlob(InvalidInput(), InvalidConnection(), options, CancellationToken.None).GetAwaiter().GetResult();
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = true;
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(() =>
            AzureBlobStorage.UploadBlob(InvalidInput(), InvalidConnection(), options, CancellationToken.None).GetAwaiter().GetResult());
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Does.Contain(CustomErrorMessage));
    }
}
