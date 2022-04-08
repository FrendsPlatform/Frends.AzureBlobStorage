# Frends.AzureBlobStorage.DownloadBlob
FRENDS Task for Downloading a blob from Azure.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Build](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions/workflows/DownloadBlob_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.AzureBlobStorage.DownloadBlob)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.AzureBlobStorage/Frends.AzureBlobStorage.DownloadBlob|main)

- [Installing](#installing)
- [Task](#task)
     - [DownloadBlob](#DownloadBlob)
- [Building](#building)
- [License](#license)
- [Contributing](#contributing)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed 'Insert nuget feed here'.

# Task

## DownloadBlob
Downloads a blob to a file.

### Properties

| Property            | Type                           | Description                                                                                                                 | Example                      |
|---------------------|--------------------------------|-----------------------------------------------------------------------------------------------------------------------------|------------------------------|
| Connection String   | `string`                       | Connection string to Azure storage                                                                                          | 'UseDevelopmentStorage=true' |
| Container Name      | `string`                       | Name of the azure blob storage container from where the data will be downloaded.                                            | 'my-container'               |
| Blob Name           | `string`                       | Name of the blob to be downloaded.                                                                                          | 'downloadMe.xml'             |
| Directory           | `string`                       | Download destination directory.                                                                                             | 'c:\downloads'               |
| FileExistsOperation | enum<Error, Rename, Overwrite> | **Error**: Throws exception. **Overwrite**: Writes over existing file. **Rename**: Renames file by adding '(1)' at the end. | Error                        |

### Returns

Task returns an object with following properties

| Property  | Type     | Description                   | Example             |
|-----------|----------|-------------------------------|---------------------|
| FileName  | `string` | Downloaded file name.         | testfile.txt        |
| Directory | `string` | Download directory.           | tmp                 |
| FullPath  | `string` | Full path to downloaded file. | c:\tmp\testfile.txt |

# Building

Clone a copy of the repo.

`git clone https://github.com/FrendsPlatform/Frends.AzureBlobStorage`

Go to the task directory.

`cd Frends.AzureBlobStorage/Frends.AzureBlobStorage.DownloadBlob`

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
