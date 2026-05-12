# Changelog

## [3.0.0] - 2026-05-04

### Changed

- [Breaking] Replaced `Source` and `Destination` parameter tabs with unified `Input` and `Options` tabs.
- [Breaking] `Input.ActionOnExistingFile` replaces `Destination.FileExistsOperation`; enum value `Error` renamed to `Throw`.
- [Breaking] `Input.TargetDirectory` replaces `Destination.Directory`.
- [Breaking] `Options.Encoding` replaces `Source.Encoding`; added `UTF8WithBOM` value; `WINDOWS1252` renamed to `Windows1252`.
- [Breaking] `Options.OtherEncoding` replaces `Source.FileEncodingString`; separate `EnableBOM` property removed (use `UTF8WithBOM`).
- [Breaking] `Result.FilePath` replaces `Result.FullPath`; `Result.FileName` and `Result.Directory` removed.

### Added

- Added `Result.Success` (bool) and `Result.Error` properties.
- Added `Options.ThrowErrorOnFailure` (default `true`) and `Options.ErrorMessageOnFailure` for non-throwing error handling.

## [2.0.0] - 2026-04-26

### Changed

- Standardized parameter names and validation across all Azure Blob Storage tasks for consistency.

## [1.7.0] - 2026-01-28

### Added

- Add options to support Arc Managed Identity authentication.

## [1.6.0] - 2026-01-27

### Added

- Added TargetFileName property to Destination for custom file naming when downloading blobs.

## [1.5.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [1.4.0] - 2025-02-17

### Added

- Added SAS Token authentication method.

## [1.3.0] - 2024-08-21

### Changed

- Updated Azure.Identity to version 1.12.0.
- Removed dependencies Azure.Core and Azure.Storage.Common.

## [1.2.0] - 2024-01-31

### Updated

- Azure.Identity to version 1.10.4
- Azure.Storage.Blobs to version 12.19.1
- Azure.Storage.Common to version 12.18.1
- Azure.Core to version 1.36.0
- MimeMapping to version 1.0.1.50

### Fixed

- [Breaking] Fixed Source parameters to be similar to the UploadFiles Task.

## [1.1.1] - 2023-02-08

### Fixed

- Fixed issue with empty encoding parameter.
- Memory leak fix by unloading assembly context after Task execution.

## [1.1.0] - 2022-12-01

### Added

- OAuth autentication method

### Changed

- Security updated for dependency:
  - Azure.Storage.Blobs 12.13.1 to 12.14.1
  - Azure.Storage.Common 12.12.0 to 12.13.0
  - Azure.Core 1.25.0 to 1.26.0

## [1.0.2] - 2022-09-02

### Changed

- Security updated for dependency:
  - Azure.Storage.Blobs 12.10.0 to 12.13.1
  - Also updated dependencies:
  - Azure.Storage.Common 12.9.0 to 12.12.0
  - Azure.Core 1.20.0 to 1.25.0
  - System.ComponentModel.Annotations 4.7.0 to 5.0.0

## [1.0.1] - 2022-02-28

### Changed

- Support for .NET Standard 2.0 removed.

## [1.0.0] - 2022-02-09

### Added

- Initial implementation of Frends.AzureBlobStorage.DownloadBlob.
