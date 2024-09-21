# BLOB Storing Azure Provider

BLOB Storing Azure Provider can store BLOBs in [Azure Blob storage](https://azure.microsoft.com/en-us/services/storage/blobs/).

## Installation

Install the `Abp.BlobStoring.Azure` NuGet package to your project and add `[DependsOn(typeof(AbpBlobStoringAzureModule))]` to the ABP module class inside your project.

## Configuration

Configuration is done in the Initialize method of your module class.

Example: Configure to use the azure storage provider by default

```csharp
Configuration.Modules.AbpBlobStoring().Containers.Configure<AbpBlobStoringOptions>(options =>
{
    options.Containers.ConfigureDefault(container =>
    {
        container.UseAzure(azure =>
        {
            azure.ConnectionString = "your azure connection string";
            azure.ContainerName = "your azure container name";
            azure.CreateContainerIfNotExists = true;
        });
    });
});
```

### Options

* **ConnectionString (string)**: A connection string includes the authorization information required for your application to access data in an Azure Storage account at runtime using Shared Key authorization. Please refer to Azure documentation: https://docs.microsoft.com/en-us/azure/storage/common/storage-configure-connection-string
* **ContainerName (string)**: You can specify the container name in azure. If this is not specified, it uses the name of the BLOB container defined with the BlobContainerName attribute (see the BLOB storing document). Please note that Azure has some rules for naming containers. A container name must be a valid DNS name, conforming to the following naming rules:
    * Container names must start or end with a letter or number, and can contain only letters, numbers, and the dash (-) character.
    * Every dash (-) character must be immediately preceded and followed by a letter or number; consecutive dashes are not permitted in container names.
    * All letters in a container name must be lowercase.
    * Container names must be from 3 through 63 characters long.
* **CreateContainerIfNotExists (bool)**: Default value is false, If a container does not exist in azure, AzureBlobProvider will try to create it.
