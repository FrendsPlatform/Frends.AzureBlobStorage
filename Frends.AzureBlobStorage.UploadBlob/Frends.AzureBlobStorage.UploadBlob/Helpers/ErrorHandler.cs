using System;
using Frends.AzureBlobStorage.UploadBlob.Definitions;

namespace Frends.AzureBlobStorage.UploadBlob.Helpers;

/// <summary>
/// Converts an exception into a failed Result object or rethrows based on task options.
/// </summary>
internal static class ErrorHandler
{
    /// <param name="exception">The exception to handle.</param>
    /// <param name="options">Task options that control whether failures are returned as a Result object or thrown.</param>
    /// <param name="throwCanceled">
    /// When true, an OperationCanceledException is rethrown immediately.
    /// When false, cancellation is handled like any other failure.
    /// </param>
    /// <returns>A failed Result object when the exception is handled instead of rethrown.</returns>
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
            throw new Exception(exception.Message, exception);

        throw new Exception(customMessage, exception);
    }

    private static Result ReturnResult(Exception exception, string customMessage = null)
    {
        var errorMessage = string.IsNullOrEmpty(customMessage)
            ? exception.Message
            : $"{customMessage}: {exception.Message}";

        return new Result(
            false,
            null,
            new Error
            {
                Message = errorMessage,
                AdditionalInfo = exception,
            });
    }
}
