# Changelog

## [3.0.0] - 2026-04-17

### Changed

- [Breaking] Renamed `Source` tab to `Input` tab; move task parameter references from `Source` to `Input` when upgrading.
- Added `Options.ThrowErrorOnFailure` (default: `true`) — when `false`, errors are returned in the result instead of thrown.
- Added `Options.ErrorMessageOnFailure` — custom error message prefix for thrown exceptions or returned errors.
- Added `Result.Success` (bool) — `true` on success, `false` on failure.
- Added `Result.Error` (object `{ string Message, object AdditionalInfo }`) — populated on failure when `ThrowErrorOnFailure` is `false`.

#### Upgrade instructions

- Replace any usage of the `Source` tab/class with `Input`. The properties `ContainerName` and `BlobName` remain unchanged.

## [2.0.0] - 2026-04-26

### Changed

- Standardized parameter names and validation across all Azure Blob Storage tasks for consistency.

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
