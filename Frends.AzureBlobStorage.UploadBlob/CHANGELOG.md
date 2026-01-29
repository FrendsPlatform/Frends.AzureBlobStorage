# Changelog

## [3.6.0] - 2026-01-23

### Added

- Add options to support Arc Managed Identity authentication.

## [3.5.0] - 2026-01-15

### Changed

- Updated Azure packages to the latest versions:
- Azure.Storage.Blobs 12.27.0
- Azure.Identity 1.17.1

## [3.4.0] - 2025-11-12

- Added note about OAuth2 requiring Storage Blob Data Owner role for Tags property

## [3.3.0] - 2025-11-12

### Changed

- Updated stream handling in to improve performance and reduce memory usage during large file uploads.

## [3.2.0] - 2025-11-01

### Changed

- Performance improvements

## [3.1.0] - 2025-08-26

### Changed

- Updated the UploadBlob task to use a streaming approach (GetStream) instead of reading the entire file into memory (
  GetBytes), added unit tests to verify integrity for small and 200MB files.

## [3.0.0] - 2025-04-16

### Changed

- [Breaking] Reorganized and renamed parameters for clarity and consistency
  - To upgrade to the new version you can select the new parameters matching the old ones.  
    You can find a list of the changes to parameter locations and names below:
    - Renamed Source parameter tab to Input
    - Renamed Destination parameter tab to Connection
    - Destination.HandleExistingFile renamed and moved to Input.ActionOnExistingFile
    - HandleExistingFile.Error renamed to OnExistingFile.Throw
    - ConnectionMethod.SASToken renamed to ConnectionMethod.SasToken
    - Destination.ApplicationID renamed and moved to Connection.ApplicationId
    - Destination.TenantID renamed and moved to Connection.TenantId
    - Destination.SASToken renamed and moved to Connection.SasToken
    - Destination.BlobType moved to Options.BlobType
    - Destination.ResizeFile moved to Options.ResizeFile
    - Destination.PageMaxSize moved to Options.PageMaxSize
    - Destination.PageOffset moved to Options.PageOffset
    - Destination.ContentType moved to Options.ContentType
    - Destination.Encoding moved to Options.Encoding
    - FileEncoding.WINDOWS1252 renamed to Windows1252
    - Destination.EnableBOM renamed and moved to Options.EnableBom
    - Destination.FileEncodingString moved to Options.FileEncodingString
    - Destination.ParallelOperations moved to Options.ParallelOperations

## [2.4.0] - 2025-01-27

### Added

- Added SAS Token authentication method.

## [2.3.0] - 2024-12-11

### Updated

- Removed progress handler

## [2.2.0] - 2024-12-05

### Updated

- Fixed GZip compression

## [2.1.0] - 2024-08-21

### Updated

- Updated Azure.Identity to version 1.12.0.

## [2.0.1] - 2024-01-29

### Updated

- Azure.Identity to version 1.10.4
- Azure.Storage.Blobs to version 12.19.1

## [2.0.0] - 2023-04-06

### Added

Option to choose whether to upload a directory or just a single blob.
New parameters: Destination.ResizeFile, Source.SourceType, Source.SourceDirectory, Source.SearchPattern,
Source.BlobName, Source.BlobFolderName.

### Changed

Destination.BlobName and Destination.RenameTo parameters have been replaced by Source.BlobName or Source.BlobFolderName
depending on Source.SourceType.
Result.SourceFile and Result.Uri have been replaced by Result.Data.

## [1.2.0] - 2023-01-27

### Added

- New feature to add index tags to uploaded blobs (Block and Append).

## [1.1.0] - 2023-01-03

### Added

- OAuth2 as a new additional authentication method.

## [1.0.2] - 2022-07-29

### Added

- Append any type of blob. Fixes for Page and Append blob upload.

## [1.0.1] - 2022-02-28

### Changed

- Support for .NET Standard 2.0 removed.

## [1.0.0] - 2022-02-09

### Added

- Initial implementation of Frends.AzureBlobStorage.UploadBlob.
