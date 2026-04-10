using System;
using dotenv.net;

namespace Frends.AzureBlobStorage.ListBlobsInContainer.Tests;

internal static class TestHelper
{
    internal static string StorageAccountName;
    internal static string ConnectionString;
    internal static string TenantId;
    internal static string ClientId;
    internal static string ClientSecret;
    internal static string AccessKey;

    internal static void LoadEnvironmentVariables()
    {
        DotEnv.Load();
        StorageAccountName = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_StorageAccount");
        ConnectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
        TenantId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
        ClientId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
        ClientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
        AccessKey = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_frendstaskstestcontainerAccessKey");
    }
}
