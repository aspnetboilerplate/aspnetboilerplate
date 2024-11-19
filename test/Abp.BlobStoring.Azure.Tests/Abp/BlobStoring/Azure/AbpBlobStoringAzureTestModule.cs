using System;
using System.Reflection;
using Abp.BlobStoring.Tests.Abp.BlobStoring;
using Abp.Modules;
using Azure.Storage.Blobs;

namespace Abp.BlobStoring.Azure.Tests.Abp.BlobStoring.Azure;

/// <summary>
/// This module will not try to connect to azure.
/// </summary>
[DependsOn(
    typeof(AbpBlobStoringAzureModule),
    typeof(AbpBlobStoringTestModule)
)]
public class AbpBlobStoringAzureTestCommonModule : AbpModule
{

}

[DependsOn(
    typeof(AbpBlobStoringAzureTestCommonModule)
)]
public class AbpBlobStoringAzureTestModule : AbpModule
{
    private const string UserSecretsId = "9f0d2c00-80c1-435b-bfab-2c39c8249091";

    private string _connectionString;

    private readonly string _randomContainerName = "abp-azure-test-container-" + Guid.NewGuid().ToString("N");

    public override void PreInitialize()
    {
        //Configuration.Services.ReplaceConfiguration(ConfigurationHelper.BuildConfiguration(builderAction: builder =>
        //{
        //    builder.AddUserSecrets(UserSecretsId);
        //}));

        //Configuration.ReplaceService<BlobServiceClient, NullBlobServiceClient>(DependencyLifeStyle.Transient);
        _connectionString = "";
        Configuration.Modules.AbpBlobStoring().Containers.ConfigureAll((containerName, containerConfiguration) =>
        {
            containerConfiguration.UseAzure(azure =>
            {
                azure.ConnectionString = _connectionString;
                azure.ContainerName = _randomContainerName;
                azure.CreateContainerIfNotExists = true;
            });
        });
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }

    public override void Shutdown()
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        blobServiceClient.GetBlobContainerClient(_randomContainerName).DeleteIfExists();

        base.Shutdown();
    }
}