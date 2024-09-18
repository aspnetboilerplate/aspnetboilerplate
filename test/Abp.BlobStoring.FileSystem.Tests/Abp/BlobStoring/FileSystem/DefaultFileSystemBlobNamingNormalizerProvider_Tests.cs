using System.Runtime.InteropServices;
using Abp.Runtime.System;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.BlobStoring.FileSystem.Tests.Abp.BlobStoring.FileSystem;

public class DefaultFileSystemBlobNamingNormalizerProvider_Tests : AbpBlobStoringFileSystemTestBase
{
    private readonly IBlobNamingNormalizer _blobNamingNormalizer;

    public DefaultFileSystemBlobNamingNormalizerProvider_Tests()
    {
        _blobNamingNormalizer = Resolve<IBlobNamingNormalizer>();
    }

    protected override void PostInitialize()
    {
        base.PostInitialize();
        var _iosPlatformProvider = Substitute.For<IOSPlatformProvider>();
        _iosPlatformProvider.GetCurrentOSPlatform().Returns(OSPlatform.Windows);

        LocalIocManager.IocContainer.Register(
            Component.For<IOSPlatformProvider>()
                .UsingFactoryMethod(() => _iosPlatformProvider)
                .LifestyleSingleton()
        );
    }

    [Fact]
    public void NormalizeContainerName()
    {
        var filename = "thisismy:*?\"<>|foldername";
        filename = _blobNamingNormalizer.NormalizeContainerName(filename);
        filename.ShouldBe("thisismyfoldername");
    }

    [Fact]
    public void NormalizeBlobName()
    {
        var filename = "thisismy:*?\"<>|filename";
        filename = _blobNamingNormalizer.NormalizeContainerName(filename);
        filename.ShouldBe("thisismyfilename");
    }

    [Theory]
    [InlineData("/")]
    [InlineData("\\")]
    public void NormalizeContainerName_With_Directory(string delimiter)
    {
        var filename = $"thisis{delimiter}my:*?\"<>|{delimiter}foldername";
        filename = _blobNamingNormalizer.NormalizeContainerName(filename);
        filename.ShouldBe($"thisis{delimiter}my{delimiter}foldername");
    }

    [Theory]
    [InlineData("/")]
    [InlineData("\\")]
    public void NormalizeBlobName_With_Directory(string delimiter)
    {
        var filename = $"thisis{delimiter}my:*?\"<>|{delimiter}filename";
        filename = _blobNamingNormalizer.NormalizeContainerName(filename);
        filename.ShouldBe($"thisis{delimiter}my{delimiter}filename");
    }
}