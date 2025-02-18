# Changelog

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
