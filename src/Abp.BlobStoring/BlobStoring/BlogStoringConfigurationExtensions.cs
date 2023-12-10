using Abp.Configuration.Startup;

namespace Abp.BlobStoring
{
    public static class BlogStoringConfigurationExtensions
    {
        public static AbpBlobStoringOptions AbpBlobStoring(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<AbpBlobStoringOptions>();
        }
    }
}
