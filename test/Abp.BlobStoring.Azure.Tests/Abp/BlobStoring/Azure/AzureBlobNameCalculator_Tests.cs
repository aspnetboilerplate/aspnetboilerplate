using Abp.Configuration.Startup;
using Shouldly;
using Xunit;

namespace Abp.BlobStoring.Azure.Tests.Abp.BlobStoring.Azure;

public class AzureBlobNameCalculator_Tests : AbpBlobStoringAzureTestCommonBase
{
    private readonly IAzureBlobNameCalculator _calculator;

    private const string AzureContainerName = "/";
    private const string AzureSeparator = "/";

    public AzureBlobNameCalculator_Tests()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;
        AbpSession.TenantId = null;

        _calculator = Resolve<IAzureBlobNameCalculator>();
    }

    [Fact]
    public void Default_Settings()
    {
        _calculator.Calculate(
            GetArgs("my-container", "my-blob")
        ).ShouldBe($"host{AzureSeparator}my-blob");
    }

    [Fact]
    public void Default_Settings_With_TenantId()
    {
        var tenantId = RandomHelper.GetRandom();

        using (AbpSession.Use(tenantId, null))
        {
            _calculator.Calculate(
                GetArgs("my-container", "my-blob")
            ).ShouldBe($"tenants{AzureSeparator}{tenantId:D}{AzureSeparator}my-blob");
        }
    }

    private static BlobProviderArgs GetArgs(
        string containerName,
        string blobName)
    {
        return new BlobProviderGetArgs(
            containerName,
            new BlobContainerConfiguration().UseAzure(x =>
            {
                x.ContainerName = containerName;
            }),
            blobName
        );
    }
}