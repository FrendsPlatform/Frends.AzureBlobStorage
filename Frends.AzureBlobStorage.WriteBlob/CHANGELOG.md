# Changelog

## [3.0.0] - 2026-08-17

### Changed

- [Breaking Change] Combined `Source` and `Destination` parameters into a single `Input` parameter to align with Frends Task platform standards.
- Added `ErrorMessageOnFailure` to `Options` to allow customizing the error message when the Task fails.
- When `ThrowErrorOnFailure` is `false`, the returned `Result` now includes an `Error` property with details about what went wrong instead of silently returning a failed result.

## [2.0.0] - 2026-04-26

### Changed

- Standardized parameter names and validation across all Azure Blob Storage tasks for consistency.

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
