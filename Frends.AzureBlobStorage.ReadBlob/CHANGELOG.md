# Changelog

## [2.0.0] - 2026-03-24

### Added
- Added support for Default Managed Identity.
 
### Changed

- [Breaking] Reorganized and renamed parameters for clarity and consistency and updated dotnet SDK to version 8.
  - To upgrade to the new version you can select the new parameters matching the old ones.
    You can find a list of the changes to parameter locations and names below:
	- Source.ConnectionMethod moved to Connection.AuthenticationMethod
	- Source.ContainerName moved to Connection.ContainerName
	- Source.ConnectionString moved to Connection.ConnectionString
	- Source.ApplicationID renamed moved to Connection.ApplicationId
	- Source.TenantID renamed moved to Connection.TenantId
	- Source.ClientSecret moved to Connection.ClientSecret
	- Source.Scopes renamed moved to Connection.Scopes
	- Source.TargetTenantId moved to Connection.TargetTenantId
	- Source.TargetClientId moved to Connection.TargetClientId
	- Source.StorageAccountName removed, use Connection.Uri
	- Source.BlobName moved to Input.BlobName
	- Source.Encoding moved to Input.Encoding
	- Source.EnableBOM moved to Input.EnableBOM
	- Source.FileEncodingString moved to Input.FileEncodingString
- Updated dotnet SDK to version 8.

## [1.4.0] - 2026-01-23

### Added

- Add options to support Arc Managed Identity authentication.

## [1.3.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [1.2.0] - 2024-08-21

### Updated

- Updated Azure.Identity to version 1.12.0.

## [1.1.1] - 2024-01-31

### Updated

- Azure.Identity to version 1.10.4
- Azure.Storage.Blobs to version 12.19.1

## [1.1.0] - 2022-12-16

### Added

- OAuth2 as a new additional authentication method.

### Changed

- Dependency update:
  Removed dependencies:
  System.ComponentModel.Annotations
  Azure.Core
  Azure.Storage.Common

  Added dependencies:
  Azure.Identity 1.8.0

  Update dependencies:
  Azure.Storage.Blobs 12.13.1 to 12.14.1

## [1.0.0] - 2022-04-08

### Added

- Initial implementation
