# BLOB Storing File System Provider

File System Storage Provider is used to store BLOBs in the local file system as standard files inside a folder.

## Installation

Install the `Abp.BlobStoring.FileSystem` NuGet package to your project and add `[DependsOn(typeof(AbpBlobStoringFileSystemModule))]` to the ABP module class inside your project.

## Configuration

Configuration is done in the Initialize method of your module class.

Example: Configure to use the File System storage provider by default

```csharp
Configuration.Modules.AbpBlobStoring().Containers.Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseFileSystem(fileSystem =>
        {
            fileSystem.BasePath = "C:\\my-files";
        });
    });
});
```

### Options

* **BasePath (string)**: The base folder path to store BLOBs. It is required to set this option.
* **AppendContainerNameToBasePath** (bool; default: true): Indicates whether to create a folder with the container name inside the base folder. If you store multiple containers in the same BaseFolder, leave this as true. Otherwise, you can set it to false if you don't like an unnecessarily deeper folder hierarchy.