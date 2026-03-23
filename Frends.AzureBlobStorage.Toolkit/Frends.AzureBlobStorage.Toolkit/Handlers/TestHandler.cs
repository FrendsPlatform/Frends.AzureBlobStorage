using System;
using dotenv.net;

namespace Frends.AzureBlobStorage.Toolkit.Handlers;

#pragma warning disable SA1600 // self explanatory
public static class TestHandler
{
#pragma warning disable SA1401 // fields are public to use in outer projects
#pragma warning disable CA2211 // fields are public to use in outer projects
    public static string StorageAccountName;
    public static string ConnectionString;
    public static string TenantId;
    public static string ClientId;
    public static string ClientSecret;
    public static string SasToken;
    public static string AccessKey;
#pragma warning restore SA1401
#pragma warning restore CA2211

    public static void LoadEnvironmentVariables()
    {
        DotEnv.Load();
        StorageAccountName = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_StorageAccount");
        ConnectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
        TenantId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
        ClientId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
        ClientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
        SasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");
        AccessKey = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_frendstaskstestcontainerAccessKey");
    }
}
#pragma warning restore SA1600
