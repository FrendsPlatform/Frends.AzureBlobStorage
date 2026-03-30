# Frends.AzureBlobStorage

Frends Tasks for Azure Blob Storage operations.

# Tasks

- [Frends.AzureBlobStorage.DownloadBlob](Frends.AzureBlobStorage.DownloadBlob/README.md)
- [Frends.AzureBlobStorage.UploadBlob](Frends.AzureBlobStorage.UploadBlob/README.md)
- [Frends.AzureBlobStorage.DeleteBlob](Frends.AzureBlobStorage.DeleteBlob/README.md)
- [Frends.AzureBlobStorage.ReadBlob](Frends.AzureBlobStorage.ReadBlob/README.md)
- [Frends.AzureBlobStorage.ListBlobsInContainer](Frends.AzureBlobStorage.ListBlobsInContainer/README.md)
- [Frends.AzureBlobStorage.CreateContainer](Frends.AzureBlobStorage.CreateContainer/README.md)
- [Frends.AzureBlobStorage.DeleteContainer](Frends.AzureBlobStorage.DeleteContainer/README.md)
- [Frends.AzureBlobStorage.ListContainers](Frends.AzureBlobStorage.ListContainers/README.md)

# Building NuGet Packages

`Pack-AllProjects.ps1` (located in the repository root) builds Release NuGet packages for every `Frends.AzureBlobStorage.*` project, excluding test projects.

**Requirements:** PowerShell 5.1 or later and the .NET SDK must be available on `PATH`.

## Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `-RootPath` | `string` | Script directory | Root directory searched recursively for `*.csproj` files. |
| `-OutputDir` | `string` | `C:\Temp\AzureBlob` | Directory where the `.nupkg` files are written. Created automatically if it does not exist. |
| `-BumpVersion` | `switch` | _(off)_ | When specified, increments the patch segment of `<Version>` in each project file before packing (e.g. `1.2.3` ? `1.2.4`). Pre-release suffixes such as `-beta` are preserved. |

## Examples

Pack all projects using their current versions:

```powershell
.\Pack-AllProjects.ps1
```

Pack and write packages to a custom output folder:

```powershell
.\Pack-AllProjects.ps1 -OutputDir "D:\MyPackages"
```

Bump each project's patch version, then pack:

```powershell
.\Pack-AllProjects.ps1 -BumpVersion
```

Bump the patch version and write packages to a custom folder:

```powershell
.\Pack-AllProjects.ps1 -BumpVersion -OutputDir "D:\MyPackages"
```


# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repository on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!
