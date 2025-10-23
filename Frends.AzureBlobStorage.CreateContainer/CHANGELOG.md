# Changelog

## [2.0.0] - 2025-10-23

### Added
- Created new **Connection** tab to organize authentication and connection details.
- Created new **Options** tab with `ThrowErrorOnFailure` and `ErrorMessageOnFailure` fields.
- Added structured **Error** object (`Message`, `AdditionalInfo`) to task result.

### Changed
- Moved connection-related parameters from **Input** to **Connection** tab.
- Renamed:
  - `Input.ConnectionMethod` → `Connection.AuthenticationMethod`
  - `Input.ApplicationID` → `Connection.ApplicationId`
  - `Input.TenantID` → `Connection.TenantId`
- Updated task logic and unit tests to align with new tab structure and error handling.

### Breaking changes
- Parameter moves and renames make this a **breaking change**.
- To upgrade:
  - Move all connection fields to the **Connection** tab.
  - Add **Options.ThrowErrorOnFailure** and **Options.ErrorMessageOnFailure** to the task configuration.


## [1.3.0] - 2024-08-21
### Updated
- Updated Azure.Identity to version 1.12.0.

## [1.2.0] - 2024-01-31
### Updated
- Azure.Identity to version 1.10.4
- Azure.Storage.Blobs to version 12.19.1
## Fixed
- Modified Input parameters to be similar to other AzureBlobStorage Tasks.

## [1.1.0] - 2022-11-28
### Added
- OAuth autentication
### Modified
- Unnecessary packages removed:
	- Azure.Core 1.20.0
	- MimeMapping 1.0.1.37
	- Microsoft.CSharp 4.7.0
	- System.ComponentModel.Annotations 4.7.0
- Task's result modified: Result.Success added. 
- Documentation updated.

## [1.0.0] - 2022-11-04
### Added
- Initial implementation of Frends.AzureBlobStorage.CreateContainer.
