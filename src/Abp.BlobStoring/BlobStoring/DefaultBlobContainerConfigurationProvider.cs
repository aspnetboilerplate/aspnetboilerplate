using Abp.Dependency;

namespace Abp.BlobStoring
{
    public class DefaultBlobContainerConfigurationProvider : IBlobContainerConfigurationProvider, ITransientDependency
    {
        protected AbpBlobStoringOptions Options { get; }

        public DefaultBlobContainerConfigurationProvider(AbpBlobStoringOptions options)
        {
            Options = options;
        }

        public virtual BlobContainerConfiguration Get(string name)
        {
            return Options.Containers.GetConfiguration(name);
        }
    }
}