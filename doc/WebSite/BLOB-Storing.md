# BLOB Storing

Storing binary objects a.k.a. [BLOB](https://en.wikipedia.org/wiki/Binary_large_object)s is a feature most modern apps need. ASP.NET Boilerplate provides an infrastructure for storing BLOBs.
BLOBs can be stored in a file system, database or on a cloud provider.

ASP.NET Boilerplate provides an abstraction to work with BLOBs and provides some pre-built storage providers that you can easily integrate to. Having such an abstraction has some benefits;

* You can easily integrate to your favorite BLOB storage provides with a few lines of configuration.
* You can then easily change your BLOB storage without changing your application code.
* If you want to create reusable application modules, you don't need to make assumption about how the BLOBs are stored.

## BLOB Storage Providers

ASP.NET Boilerplate offers BLOB providers below out of the box;

* [File System](BLOB-Storing-File-System.md): Stores BLOBs in a folder of the local file system, as standard files.
* [Azure](Blob-Storing-Azure.md):  Stores BLOBs on the [Azure BLOB storage](https://azure.microsoft.com/en-us/services/storage/blobs/).

## Installation

[Abp.BlobStoring](https://www.nuget.org/packages/Abp.BlobStoring) is the main package that defines the BLOB storing services. You can use this package to use the BLOB Storing system without depending a specific storage provider.

Install the `Abp.BlobStoring` NuGet package to your project and add [DependsOn(typeof(AbpBlobStoringModule))] to the ABP module class inside your project.

## The IBlobContainer

`IBlobContainer` is the main interface to read and write BLOBs. An application may have multiple containers and each container can be separately configured. But, there is a default container that can be simply used by injecting the IBlobContainer.

Example: Simply read and write bytes of a named BLOB;

```csharp
namespace DemoApp
{
    public class MyService : ITransientDependency
    {
        private readonly IBlobContainer _blobContainer;

        public MyService(IBlobContainer blobContainer)
        {
            _blobContainer = blobContainer;
        }

        public async Task SaveBytesAsync(byte[] bytes)
        {
            await _blobContainer.SaveAsync("sample-blob", bytes);
        }
        
        public async Task<byte[]> GetBytesAsync()
        {
            return await _blobContainer.GetAllBytesOrNullAsync("sample-blob");
        }
    }
}
```

This service saves the given bytes with the `sample-blob` name and then gets the previously saved bytes with the same name.
`IBlobContainer` can work with `Stream` and `byte[]` objects, which will be detailed in the next sections.

### Saving BLOBs

`SaveAsync` method is used to save a new BLOB or replace an existing BLOB. `SaveAsync` gets the following parameters:

* **name (string):** Unique name of the BLOB.
* **stream (Stream) or bytes (byte[]):** The stream to read the BLOB content or a byte array.
* **overrideExisting (bool):** Set true to replace the BLOB content if it does already exists. Default value is false and throws BlobAlreadyExistsException if there is already a BLOB in the container with the same name.

### Reading/Getting BLOBs

* `GetAsync`: Only gets a BLOB name and returns a Stream object that can be used to read the BLOB content. Always dispose the stream after using it. This method throws exception, if it can not find the BLOB with the given name.
* `GetOrNullAsync`: In opposite to the GetAsync method, this one returns null if there is no BLOB found with the given name.
* `GetAllBytesAsync`: Returns a byte[] instead of a Stream. Still throws exception if can not find the BLOB with the given name.
* `GetAllBytesOrNullAsync`: In opposite to the GetAllBytesAsync method, this one returns null if there is no BLOB found with the given name.

### Deleting BLOBs

`DeleteAsync` method gets a BLOB name and deletes the BLOB data. It doesn't throw any exception if given BLOB was not found. Instead, it returns a bool indicating that the BLOB was actually deleted or not, if you care about it.


### Other Methods

* `ExistsAsync` method simply checks if there is a BLOB in the container with the given name.

## Typed IBlobContainer

Typed BLOB container system is a way of creating and managing multiple containers in an application;

* **Each container is separately stored.** That means the BLOB names should be unique in a container and two BLOBs with the same name can live in different containers without effecting each other.
* **Each container can be separately configured**, so each container can use a different storage provider based on your configuration.

To create a typed container, you need to create a simple class decorated with the `BlobContainerName` attribute:

```csharp
using Volo.Abp.BlobStoring;

namespace DemoApp
{
    [BlobContainerName("profile-pictures")]
    public class ProfilePictureContainer
    {
        
    }
}
```

Once you create the container class, you can inject `IBlobContainer<T>` for your container type.

Example:

```csharp
public class ProfileAppService : ApplicationService
{
    private readonly IBlobContainer<ProfilePictureContainer> _blobContainer;

    public ProfileAppService(IBlobContainer<ProfilePictureContainer> blobContainer)
    {
        _blobContainer = blobContainer;
    }

    public async Task SaveProfilePictureAsync(byte[] bytes)
    {
        var blobName = AbpSession.ToUserIdentifier().ToString();
        await _blobContainer.SaveAsync(blobName, bytes);
    }
    
    public async Task<byte[]> GetProfilePictureAsync()
    {
        var blobName = AbpSession.ToUserIdentifier().ToString();
        return await _blobContainer.GetAllBytesOrNullAsync(blobName);
    }
}
```

### The Default Container

If you don't use the generic argument and directly inject the `IBlobContainer` (as explained before), you get the default container. Another way of injecting the default container is using `IBlobContainer<DefaultContainer>`, which returns exactly the same container.

The name of the default container is `default`.

### Named Containers

Typed containers are just shortcuts for named containers. You can inject and use the `IBlobContainerFactory` to get a BLOB container by its name:

```csharp
public class ProfileAppService : ApplicationService
{
    private readonly IBlobContainer _blobContainer;

    public ProfileAppService(IBlobContainerFactory blobContainerFactory)
    {
        _blobContainer = blobContainerFactory.Create("profile-pictures");
    }

    //...
}
```

## IBlobContainerFactory

`IBlobContainerFactory` is the service that is used to create the BLOB containers. One example was shown above.

Example: Create a container by name

```csharp
var blobContainer = blobContainerFactory.Create("profile-pictures");
```

Example: Create a container by type

```csharp
var blobContainer = blobContainerFactory.Create<ProfilePictureContainer>();
```
