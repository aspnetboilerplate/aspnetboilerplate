using System;
using System.IO;
using System.Reflection;
using Abp.BlobStoring.Tests.Abp.BlobStoring;
using Abp.IO;
using Abp.Modules;
using Abp.Threading;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.BlobStoring.FileSystem.Tests.Abp.BlobStoring.FileSystem;

[DependsOn(
    typeof(AbpBlobStoringFileSystemModule),
    typeof(AbpBlobStoringTestModule)
)]
public class AbpBlobStoringFileSystemTestModule : AbpModule
{
    private readonly string _testDirectoryPath;

    public AbpBlobStoringFileSystemTestModule()
    {
        _testDirectoryPath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N")
        );
    }

    public override void PreInitialize()
    {

        //IocManager.IocContainer.Register(
        //    Component.For<IBlobNamingNormalizer>().Instance(Substitute.For<FileSystemBlobNamingNormalizer>())
        //        .LifestyleSingleton()
        //);

        Configuration
            .Modules
            .AbpBlobStoring()
            .Containers
            .ConfigureAll((containerName, containerConfiguration) =>
        {
            containerConfiguration.UseFileSystem(fileSystem =>
            {
                fileSystem.BasePath = _testDirectoryPath;
            });
        });

        base.PreInitialize();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }

    public override void Shutdown()
    {
        DirectoryHelper.DeleteIfExists(_testDirectoryPath, true);
        base.Shutdown();
    }
}