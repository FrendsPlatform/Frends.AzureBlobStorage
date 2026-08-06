# Changelog

## [2.1.0] - 2026-08-06

### Changed

- The task now returns a result with `Success` and `Error` fields, making it easier to handle failures gracefully.
- Added `ThrowErrorOnFailure` and `ErrorMessageOnFailure` options so you can choose whether the task throws an error or returns an error result when something goes wrong.
- Renamed the input parameter group from `Source` to `Input` to align with Frends task conventions.

## [2.0.0] - 2026-04-26

### Changed

- Standardized parameter names and validation across all Azure Blob Storage tasks for consistency.

## [1.3.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [1.2.0] - 2024-08-21

### Updated

- Updated Azure.Identity to version 1.12.0.

### Changed

- Fixed outdated environment variable names in tests and workflows.

## [1.1.2] - 2024-01-17

### Updated

- Azure.Identity from 1.8.1 to 1.10.2
- Azure.Storage.Blobs from 12.11.0 to 12.13.0

### Fixed

- Documentation fixes.
- Changed the Task to throw ArgumentException instead of generic Exception.

## [1.1.1] - 2023-12-11

### Added

- Result.CreatedOn.
- Result.LastModified.

## [1.1.0] - 2023-02-01

### Added

- OAuth2 authentication method.

### Changed

- Rename: Result.URI to Result.URL.

### Fixed

- Memory leak fix by unloading assembly context after Task execution.
- Result.URL returns complete URL to each blob.

## [1.0.0] - 2022-04-14

### Added

- Initial implementation
