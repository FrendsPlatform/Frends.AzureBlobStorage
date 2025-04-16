# Changelog

## [3.0.0] - 2025-04-16
### Changed
- Renamed Source parameter tab to Input
- Renamed Destination parameter tab to Connection
- Reorganized and renamed parameters for clarity and consistency
  - Destination.HandleExistingFile renamed and moved to Input.ActionOnExistingFile
    - Error option renamed to Throw
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
New parameters: Destination.ResizeFile, Source.SourceType, Source.SourceDirectory, Source.SearchPattern, Source.BlobName, Source.BlobFolderName.
### Changed
Destination.BlobName and Destination.RenameTo parameters have been replaced by Source.BlobName or Source.BlobFolderName depending on Source.SourceType.
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
