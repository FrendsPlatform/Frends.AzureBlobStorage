# Frends.AzureBlobStorage.UploadBlob
FRENDS Task for uploading a file to Azure.

[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) 
[![Build](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions/workflows/UploadBlob_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.AzureBlobStorage/actions)
![MyGet](https://img.shields.io/myget/frends-tasks/v/Frends.AzureBlobStorage.UploadBlob)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.AzureBlobStorage/Frends.AzureBlobStorage.UploadBlob|main)

- [Installing](#installing)
- [Task](#task)
     - [UploadBlob](#UploadBlob)
- [Building](#building)
- [License](#license)
- [Contributing](#contributing)
- [Changelog](#changelog)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed 'Insert nuget feed here'.

# Task

## UploadBlob
Uploads file to a target container. If the container doesn't exist, it will be created before the upload operation.

### Properties

| Property                              | Type                        | Description                                                                                                                                          | Example                      |
|---------------------------------------|-----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------|------------------------------|
| Source File                           | `string`                    | Full path to file that is uploaded.                                                                                                                  | 'c:\temp\uploadMe.xml'       |
| Contents Only                         | `bool`                      | Reads file content as string and treats content as selected encoding.                                                                                | true                         |
| Compress                              | `bool`                      | Applies GZIP compression to file or file content.                                                                                                    | true                         |
| Connection String                     | `string`                    | Connection string to Azure storage.                                                                                                                  | 'UseDevelopmentStorage=true' |
| Container Name                        | `string`                    | Name of the azure blob storage container where the data will be uploaded. If the container doesn't exist, then it will be created.                   | 'my-container'               |
| Create container if it does not exist | bool                        | Tries to create the container if it does not exist.                                                                                                  | false                        |
| Blob Type                             | enum<Append, Block or Page> | Azure blob type to upload.                                                                                                                           | Block                        |
| Rename To                             | `string`                    | If value is set, uploaded file will be renamed to this.                                                                                              | 'newFileName.xml'            |
| Overwrite                             | `bool`                      | Should upload operation overwrite existing file with same name.                                                                                      | true                         |
| ParallelOperations                    | `int`                       | The number of the concurrent operations.                                                                                                             | 64                           |
| Content-Type                          | `string`                    | Forces any content-type to file. If empty, tries to guess based on extension and MIME-type.                                                          | text/xml                     |
| Content-Encoding                      | `string`                    | File content is treated as this. Does not affect file encoding when Contents Only is true. If compression is enabled, Content-Type is set as 'gzip'. | utf8                         |

### Returns

Task returns an object with following properties

| Property   | Type     | Description                 | Example |
|------------|----------|-----------------------------|---------|
| SourceFile | `string` | Full path of file uploaded. |         |
| Uri        | `string` | URI to uploaded blob.       |         |

# Building

Clone a copy of the repo.

`git clone https://github.com/FrendsPlatform/Frends.AzureBlobStorage`

Go to the task directory.

`cd Frends.AzureBlobStorage/Frends.AzureBlobStorage.UploadBlob`

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

# Changelog

| Version | Changes                                                         |
|---------|-----------------------------------------------------------------|
| 1.0.0   | Initial implementation of Frends.AzureBlobStorage.UploadBlob. |
