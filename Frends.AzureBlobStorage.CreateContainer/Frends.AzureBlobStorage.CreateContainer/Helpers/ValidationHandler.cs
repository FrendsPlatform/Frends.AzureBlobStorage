using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Frends.AzureBlobStorage.CreateContainer.Helpers;

/// <summary>
/// Validates objects by their ValidationAttributes.
/// </summary>
public static class ValidationHandler
{
    /// <summary>
    /// Use this method to validate objects like Input, Connection, etc.
    /// </summary>
    /// <param name="objects">list of objects to validate</param>
    public static void Run(params object[] objects)
    {
        if (objects == null || objects.Length == 0)
            throw new ValidationException("Validation failed:\nYou must provide objects to validate");
        var validationMessage = objects.Select(obj => obj.Validate())
            .Aggregate(string.Empty, (current, message) => string.Join("\n", current, message));

        if (validationMessage.Trim() != string.Empty)
            throw new ValidationException($"Validation failed:\n{validationMessage}");
    }

    private static string Validate<T>(this T objectToValidate)
    {
        if (objectToValidate == null) return "Validated object can't be null!\n";
        var ctx = new ValidationContext(objectToValidate);
        List<ValidationResult> validateResults = [];
        Validator.TryValidateObject(objectToValidate, ctx, validateResults, true);

        return validateResults.Aggregate(
            string.Empty,
            (current, error) => string.Join("\n", current, $"{error.ErrorMessage}"));
    }
}
