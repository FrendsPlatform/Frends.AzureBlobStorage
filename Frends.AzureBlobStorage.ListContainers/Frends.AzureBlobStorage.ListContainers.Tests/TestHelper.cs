using System;
using dotenv.net;

namespace Frends.AzureBlobStorage.ListContainers.Tests;

internal static class TestHelper
{
    internal static string ConnectionString { get; private set; }

    internal static string TenantId { get; private set; }

    internal static string ClientId { get; private set; }

    internal static string ClientSecret { get; private set; }

    internal static string SasToken { get; private set; }

    internal static void LoadEnvironmentVariables()
    {
        DotEnv.Load();
        ConnectionString = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ConnString");
        TenantId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_TenantID");
        ClientId = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_AppID");
        ClientSecret = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_ClientSecret");
        SasToken = Environment.GetEnvironmentVariable("Frends_AzureBlobStorage_SASToken");
    }
}
