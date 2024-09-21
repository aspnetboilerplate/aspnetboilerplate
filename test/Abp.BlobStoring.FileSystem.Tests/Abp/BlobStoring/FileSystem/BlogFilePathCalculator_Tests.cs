using System.IO;
using Abp.Configuration.Startup;
using Shouldly;
using Xunit;

namespace Abp.BlobStoring.FileSystem.Tests.Abp.BlobStoring.FileSystem;

public class BlogFilePathCalculator_Tests : AbpBlobStoringFileSystemTestBase
{
    private readonly IBlobFilePathCalculator _calculator;

    public BlogFilePathCalculator_Tests()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;
        AbpSession.TenantId = null;

        _calculator = Resolve<IBlobFilePathCalculator>();
    }

    [Fact]
    public void Default_Settings()
    {
        var separator = Path.DirectorySeparatorChar;

        _calculator.Calculate(
            GetArgs($"C:{separator}my-files", "my-container", "my-blob")
        ).ShouldBe($"C:{separator}my-files{separator}host{separator}my-container{separator}my-blob");
    }

    [Fact]
    public void Default_Settings_With_TenantId()
    {
        var separator = Path.DirectorySeparatorChar;
        var tenantId = RandomHelper.GetRandom();

        using (AbpSession.Use(tenantId, null))
        {
            _calculator.Calculate(
                GetArgs($"C:{separator}my-files", "my-container", "my-blob")
            ).ShouldBe($"C:{separator}my-files{separator}tenants{separator}{tenantId:D}{separator}my-container{separator}my-blob");
        }
    }

    [Fact]
    public void AppendContainerNameToBasePath_Set_To_False()
    {
        var separator = Path.DirectorySeparatorChar;

        _calculator.Calculate(
            GetArgs($"C:{separator}my-files", "my-container", "my-blob", appendContainerNameToBasePath: false)
        ).ShouldBe($"C:{separator}my-files{separator}host{separator}my-blob");
    }

    private static BlobProviderArgs GetArgs(
        string basePath,
        string containerName,
        string blobName,
        bool? appendContainerNameToBasePath = null)
    {
        return new BlobProviderGetArgs(
            containerName,
            new BlobContainerConfiguration()
                .UseFileSystem(fs =>
                {
                    fs.BasePath = basePath;
                    if (appendContainerNameToBasePath.HasValue)
                    {
                        fs.AppendContainerNameToBasePath = appendContainerNameToBasePath.Value;
                    }
                }),
            blobName
        );
    }
}