# Changelog

## [2.0.0] - 2023-04-06
##Added:
Option to choose whether to upload a directory or just a single blob.
New parameters: Destination.ResizeFile, Source.SourceType, Source.SourceDirectory, Source.SearchPattern, Source.BlobName, Source.BlobFolderName.
##Changed:
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
