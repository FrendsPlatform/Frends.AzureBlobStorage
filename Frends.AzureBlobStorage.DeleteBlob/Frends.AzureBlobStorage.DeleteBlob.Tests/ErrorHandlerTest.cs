using Frends.AzureBlobStorage.DeleteBlob.Definitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Frends.AzureBlobStorage.DeleteBlob.Tests;

[TestClass]
public class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    [TestMethod]
    public async Task Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
            AzureBlobStorage.DeleteBlob(DefaultInput(), DefaultConnection(), DefaultOptions(), CancellationToken.None));

        Assert.IsNotNull(exception);
    }

    [TestMethod]
    public async Task Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;

        var result = await AzureBlobStorage.DeleteBlob(DefaultInput(), DefaultConnection(), options, CancellationToken.None);

        Assert.IsFalse(result.Success);
    }

    [TestMethod]
    public async Task Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;

        var exception = await Assert.ThrowsExceptionAsync<Exception>(() =>
            AzureBlobStorage.DeleteBlob(DefaultInput(), DefaultConnection(), options, CancellationToken.None));

        Assert.IsNotNull(exception);
        Assert.IsTrue(exception.Message.Contains(CustomErrorMessage));
    }

    private static Input DefaultInput() => new();

    private static Connection DefaultConnection() => new();

    private static Options DefaultOptions() =>
        new() { ThrowErrorOnFailure = true, ErrorMessageOnFailure = string.Empty, };
}
