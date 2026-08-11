using System;
using Frends.AzureBlobStorage.ListBlobsInContainer.Definitions;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Helpers;

/// <summary>
/// Provides centralized error handling functionality for Azure Blob Storage operations.
/// </summary>
internal static class ErrorHandler
{
    /// <summary>
    /// Handles the exception according to the task options.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    /// <param name="options">Task options that control whether failures are returned as a Result object or thrown.</param>
    /// <param name="throwCanceled">
    /// When true, an OperationCanceledException is rethrown immediately.
    /// When false, cancellation is handled like any other failure.
    /// </param>
    internal static Result Handle(this Exception exception, Options options, bool throwCanceled = true)
    {
        ThrowIfCanceled(exception, throwCanceled);
        if (options.ThrowErrorOnFailure) ThrowBaseException(exception, options.ErrorMessageOnFailure);

        return ReturnResult(exception, options.ErrorMessageOnFailure);
    }

    private static void ThrowIfCanceled(Exception exception, bool throwCanceled = true)
    {
        if (throwCanceled && exception is OperationCanceledException) throw exception;
    }

    private static void ThrowBaseException(Exception exception, string customMessage = null)
    {
        if (string.IsNullOrEmpty(customMessage))
            throw exception;

        throw new Exception(customMessage, exception);
    }

    private static Result ReturnResult(Exception exception, string customMessage = null)
    {
        var errorMessage = string.IsNullOrEmpty(customMessage)
            ? exception.Message
            : $"{customMessage}: {exception.Message}";

        return new Result
        {
            Success = false,
            BlobList = null,
            Error = new Error
            {
                Message = errorMessage,
                AdditionalInfo = exception,
            },
        };
    }
}
