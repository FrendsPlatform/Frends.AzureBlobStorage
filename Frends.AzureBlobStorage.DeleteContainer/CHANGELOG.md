# Changelog

## [3.0.0] - 2026-05-04

### Breaking changes

- Removed `Options.ThrowErrorIfContainerDoesNotExists`.
- `Result.ContainerWasDeleted` renamed to `Success`.
- `Result.Message` removed; error information is now in `Result.Error.Message`.

### Added

- `Options.ThrowErrorOnFailure` (default: `true`) to control whether the task throws on failure or returns an error result.
- `Options.ErrorMessageOnFailure` to provide a custom error message included in the exception or `Result.Error.Message`.
- `Result.Error` structured error object with `Message` and `AdditionalInfo` fields.

## [2.0.0] - 2026-04-26

### Changed

- Standardized parameter names and validation across all Azure Blob Storage tasks for consistency.

## [1.4.0] - 2026-01-26

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

## [1.1.0] - 2022-12-15

### Added

- OAuth2 as a new additional authentication method.

### Changed

- Dependency update:
  Removed dependencies:
  Azure.Storage.Common
  Azure.Core
  MimeMapping
  Microsoft.CSharp
  System.ComponentModel.Annotations

  Added dependencies:
  Azure.Identity 1.8.0

  Update dependencies:
  Azure.Storage.Blobs 12.10.0 to 12.14.1

## [1.0.0] - 2022-12-04

### Added

- Initial implementation of Frends.AzureBlobStorage.DeleteContainer.
