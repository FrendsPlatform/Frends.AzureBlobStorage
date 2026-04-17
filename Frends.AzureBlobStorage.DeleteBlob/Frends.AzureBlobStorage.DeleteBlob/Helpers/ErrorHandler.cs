using System;
using Frends.AzureBlobStorage.DeleteBlob.Definitions;

namespace Frends.AzureBlobStorage.DeleteBlob.Helpers;

internal static class ErrorHandler
{
    internal static Result Handle(Exception exception, bool throwOnFailure, string errorMessageOnFailure)
    {
        if (throwOnFailure)
        {
            if (string.IsNullOrEmpty(errorMessageOnFailure))
                throw new Exception(exception.Message, exception);

            throw new Exception(errorMessageOnFailure, exception);
        }

        var errorMessage = !string.IsNullOrEmpty(errorMessageOnFailure)
            ? $"{errorMessageOnFailure}: {exception.Message}"
            : exception.Message;

        return new Result(false, new DeleteBlobError { Message = errorMessage, AdditionalInfo = exception });
    }
}
