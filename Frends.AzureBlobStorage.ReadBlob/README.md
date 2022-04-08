# Frends.AzureBlobStorage.ReadBlob
FRENDS Task for reading a file from Azure Storage.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Build](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions/workflows/ReadBlob_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.AzureBlobStorage.ReadBlob)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.AzureBlobStorage/Frends.AzureBlobStorage.ReadBlob|main)

- [Installing](#installing)
- [Task](#task)
     - [ReadBlob](#ReadBlob)
- [Building](#building)
- [License](#license)
- [Contributing](#contributing)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed 'Insert nuget feed here'.

# Task

## ReadBlob
Reads contents of a blob.

### Properties

| Property          | Type                                  | Description                                                                                                   | Example                                  |
|-------------------|---------------------------------------|---------------------------------------------------------------------------------------------------------------|------------------------------------------|
| URI               | `string`                              | The base URI for the storage account.                                                                         | 'https://myaccount.blob.core.windows.net'|
| SAS Token         | `string`                              | A shared access signature. Grants restricted access rights to Azure Storage resources when combined with URI. | 'sp=r&st=2022-04-07T06:27:xxxx'          |
| Connection String | `string`                              | Connection string to Azure storage.                                                                           | 'UseDevelopmentStorage=true'             |
| Container Name    | `string`                              | Name of the azure blob storage container from where blob data is located.                                     | 'my-container'                           |
| BlobName          | `string`                              | Name of the blob which content is read.                                                                       | 'test.txt'                               |
| Encoding          | 'Enum<UTF8, UTF32, Unicode, ASCII>'   | Encoding name in which blob content is read.                                                                  |  UTF8                                    |

### Returns

Task returns an object with following properties

| Property   | Type     | Description   | Example |
|------------|----------|---------------|---------|
| Content    | `string` | Blob content. |         |

# Building

Clone a copy of the repo.

`git clone https://github.com/FrendsPlatform/Frends.AzureBlobStorage`

Go to the task directory.

`cd Frends.AzureBlobStorage/Frends.AzureBlobStorage.ReadBlob`

Build the solution.

`dotnet build`

Run tests.

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