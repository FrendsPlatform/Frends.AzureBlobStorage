# Changelog

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