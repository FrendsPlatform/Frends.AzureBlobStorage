# Changelog

## [2.0.0] - 2026-03-24

### Added
- Added support for Default Managed Identity.

### Changed

- [Breaking] Reorganized and renamed parameters for clarity and consistency and updated dotnet SDK to version 8.
  - To upgrade to the new version you can select the new parameters matching the old ones.
    You can find a list of the changes to parameter locations and names below:
	- Destination.ConnectionMethod moved to Connection.AuthenticationMethod
	- Destination.ContainerName moved to Connection.ContainerName
	- Destination.ConnectionString moved to Connection.ConnectionString
	- Destination.ApplicationID renamed moved to Connection.ApplicationId
	- Destination.TenantID renamed moved to Connection.TenantId
	- Destination.ClientSecret moved to Connection.ClientSecret
	- Destination.Scopes renamed moved to Connection.Scopes
	- Destination.TargetTenantId moved to Connection.TargetTenantId
	- Destination.TargetClientId moved to Connection.TargetClientId
	- Destination.StorageAccountName removed, use Connection.Uri
	- Source.SourceType moved to Input.SourceType
	- Source.ContentString moved to Input.ContentString
	- Source.Encoding moved to Input.Encoding
	- Source.EnableBOM moved to Input.EnableBOM
	- Source.FileEncodingString moved to Input.FileEncodingString
- Updated dotnet SDK to version 8.

### Fixed

- `CreateContainerIfItDoesNotExist` now works for all supported authentication methods (`ConnectionString`, `OAuth2`, `SASToken`, `ArcManagedIdentity`, `ArcManagedIdentityCrossTenant`, `DefaultManagedIdentity`).

## [1.3.0] - 2026-01-23

### Added

- Add options to support Arc Managed Identity authentication.

## [1.2.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [1.1.0] - 2025-10-07

### Fixed

- Change returned info type from 3rd party BlobContentInfo to string with json.

## [1.0.0] - 2025-01-24

### Added

- Initial implementation of Frends.AzureBlobStorage.WriteBlob.
