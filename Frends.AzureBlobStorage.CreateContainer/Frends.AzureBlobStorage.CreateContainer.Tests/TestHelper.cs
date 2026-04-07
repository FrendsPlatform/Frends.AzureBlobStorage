using System;
using dotenv.net;

namespace Frends.AzureBlobStorage.CreateContainer.Tests;

internal static class TestHelper
{
    internal static string ConnectionString;
    internal static string TenantId;
    internal static string ClientId;
    internal static string ClientSecret;

    internal static void LoadEnvironmentVariables()
    {
        DotEnv.Load();
        ConnectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
        TenantId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
        ClientId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
        ClientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
    }
}
