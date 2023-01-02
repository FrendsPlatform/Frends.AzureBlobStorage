﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Frends.AzureBlobStorage.ReadBlob.Definitions;

/// <summary>
/// Source parameters.
/// </summary>
public class Source
{
    /// <summary>
    /// Authentication method to use when connecting to Azure Storage.
    /// </summary>
    /// <example>AuthenticationMethod.ConnectionString</example>
    [DefaultValue(AuthenticationMethod.ConnectionString)]
    public AuthenticationMethod AuthenticationMethod { get; set; }

    /// <summary>
    /// The base URI for the Azure storage container.
    /// </summary>
    /// <example>https://storageaccount.blob.core.windows.net</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.SASToken)]
    public string URI { get; set; }

    /// <summary>
    /// A shared access signature for Azure storage container. Grants restricted access rights to Azure Storage resources when combined with URI.
    /// </summary>
    /// <example>sv=2021-04-10&amp;se=2022-04-10T10%3A431Z&amp;sr=c&amp;sp=l&amp;sig=ZJg983RovE%2BZXI</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.SASToken)]
    [PasswordPropertyText]
    public string SASToken { get; set; }

    /// <summary>
    /// Azure storage account's connection string.
    /// </summary>
    /// <example>"DefaultEndpointsProtocol=https;AccountName=accountname;AccountKey=Pdlrxyz==;EndpointSuffix=core.windows.net"</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.ConnectionString)]
    [PasswordPropertyText]
    public string ConnectionString { get; set; }

    /// <summary>
    /// Application (Client) ID of Azure AD Application.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    public string ApplicationID { get; set; }

    /// <summary>
    /// Tenant ID of Azure Tenant.
    /// </summary>
    /// <example>Y6b1hf2a-80e2-xyz2-qwer3h-3a7c3a8as4b7f</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    public string TenantID { get; set; }

    /// <summary>
    /// Client Secret of Azure AD Application.
    /// </summary>
    /// <example>Password!</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    [PasswordPropertyText]
    public string ClientSecret { get; set; }

    /// <summary>
    /// Name of the storage account.
    /// </summary>
    /// <example>Storager</example>
    [UIHint(nameof(AuthenticationMethod), "", AuthenticationMethod.OAuth2)]
    public string StorageAccountName { get; set; }

    /// <summary>
    /// Azure storage container's name.
    /// </summary>
    /// <example>Container1</example>
    public string ContainerName { get; set; }

    /// <summary>
    /// Name of the blob which content is read.
    /// </summary>
    /// <example>File.txt</example>
    [DisplayFormat(DataFormatString = "Text")]
    public string BlobName { get; set; }
}