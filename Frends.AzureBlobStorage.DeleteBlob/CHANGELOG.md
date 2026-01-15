# Changelog

## [1.4.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [1.3.0] - 2025-12-12

### Fixed

- Fixed typo in error message.

## [1.2.0] - 2024-08-21

### Updated

- Updated Azure.Identity to version 1.12.0.

### Changed

- Fixed outdated environment variable names in tests and workflows.

## [1.1.1] - 2024-01-24

### Updated

- Azure.Identity to version 1.10.4
- Azure.Storage.Blobs to version 12.19.1

## [1.1.0] - 2022-12-20

### Added

- OAuth2 as a new additional authentication method.
- New parameter 'Options.ThrowErrorIfBlobDoesNotExists' to choose if non existing blob throws an error or return an
  error as Result.Info.

### Changed

- New parameter 'Info' to result object.
- Dependency update:
  Removed dependencies:
  System.ComponentModel.Annotations
  Azure.Core
  Azure.Storage.Common
  MimeMapping
  Microsoft.CSharp

  Added dependencies:
  Azure.Identity 1.8.0

  Update dependencies:
  Azure.Storage.Blobs 12.13.1 to 12.14.1

## [1.0.1] - 2022-08-31

### Changed

- Security updated for dependency:
  Azure.Storage.Blobs 12.10.0 to 12.13.1
  Also updated dependencies:
  Azure.Storage.Common 12.9.0 to 12.12.0
  Azure.Core 1.20.0 to 1.25.0
  System.ComponentModel.Annotations 4.7.0 to 5.0.0

## [1.0.0] - 2022-03-07

### Added

- Initial implementation of Frends.AzureBlobStorage.DeleteBlob.