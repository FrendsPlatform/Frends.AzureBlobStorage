using System;
using Frends.Common.Toolkit.Definitions;

namespace Frends.AzureBlobStorage.CreateContainer.Definitions;

/// <summary>
/// Error information.
/// </summary>
public class Error : ITaskError
{
    /// <summary>
    /// Error message.
    /// </summary>
    /// <example>CreateContainer failed</example>
    public string Message { get; set; }

    /// <summary>
    /// Additional error information.
    /// </summary>
    /// <example>object { Exception AdditionalInfo }</example>
    public Exception AdditionalInfo { get; set; }
}
