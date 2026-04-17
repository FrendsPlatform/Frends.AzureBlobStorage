using System;
using Frends.AzureBlobStorage.ReadBlob.Definitions;

namespace Frends.AzureBlobStorage.ReadBlob.Helpers;

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

        return new Result
        {
            Success = false,
            Content = null,
            Error = new Error { Message = errorMessage, AdditionalInfo = exception }
        };
    }
}
