# Frends.AzureBlobStorage.DeleteBlob

FRENDS task for deleting blob from Azure.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Build](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions/workflows/DeleteBlob_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.AzureBlobStorage.DeleteBlob)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.AzureBlobStorage/Frends.AzureBlobStorage.DeleteBlob|main)

- [Installing](#installing)
- [Tasks](#tasks)
     - [DeleteBlob](#deleteblob)
- [Building](#building)
- [License](#license)
- [Contributing](#contributing)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed 'Insert nuget feed here'.

# Tasks

## DeleteBlob

Deletes a blob from a target container. Operation result is seen as successful even if the blob or the container doesn't exist.

### Properties

| Property                  | Type                                              | Description                                     | Example                      |
|---------------------------|---------------------------------------------------|-------------------------------------------------|------------------------------|
| Connection String         | `string`                                          | Connection string to Azure storage.             | 'UseDevelopmentStorage=true' |
| Container Name            | `string`                                          | Name of the container where delete blob exists. | 'my-container'               |
| Blob Name                 | `string`                                          | Name of the blob to delete.                     | 'deleteMe.xml'               |
| Verify ETag when deleting | `string`                                          | Delete blob only if the ETag matches.           | '0x9FE13BAA3234312'          |
| Blob Type                 | enum<Append, Block, Page>                         | Azure blob type to read.                        | Block                        |
| Snapshot delete option    | enum<None, IncludeSnapshots, DeleteSnapshotsOnly> | Defines what should be done with blob snapshots | None                         |

### Returns

Task returns an object with following properties.

| Property | Type   | Description                                           | Example |
|----------|--------|-------------------------------------------------------|---------|
| Success  | `bool` | Indicates whether the operation was succesful or not. | true    |

# Building

Clone a copy of the repo.

`git clone https://github.com/FrendsPlatform/Frends.AzureBlobStorage`

Go to the task directory.

`cd Frends.AzureBlobStorage/Frends.AzureBlobStorage.DeleteBlob`

Build the project.

`dotnet build`

Run unit test.

`dotnet test`

Create a nuget package.

`dotnet pack --configuration Release`

# License

This project is licensed under the MIT License - see the LICENSE file for details.

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!
