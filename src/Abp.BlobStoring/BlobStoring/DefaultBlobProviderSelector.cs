using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Reflection;
using Abp.Reflection.Extensions;
using JetBrains.Annotations;

namespace Abp.BlobStoring
{
    public class DefaultBlobProviderSelector : IBlobProviderSelector, ITransientDependency
    {
        protected IEnumerable<IBlobProvider> BlobProviders { get; }

        protected IBlobContainerConfigurationProvider ConfigurationProvider { get; }

        public DefaultBlobProviderSelector(
            IBlobContainerConfigurationProvider configurationProvider,
            IIocResolver iocResolver)
        {
            ConfigurationProvider = configurationProvider;
            BlobProviders = iocResolver.ResolveAll<IBlobProvider>();
        }

        [NotNull]
        public virtual IBlobProvider Get([NotNull] string containerName)
        {
            Check.NotNull(containerName, nameof(containerName));

            var configuration = ConfigurationProvider.Get(containerName);

            if (!BlobProviders.Any())
            {
                throw new AbpException("No BLOB Storage provider was registered! At least one provider must be registered to be able to use the BLOB Storing System.");
            }

            if (configuration.ProviderType == null)
            {
                throw new AbpException("No BLOB Storage provider was used! At least one provider must be configured to be able to use the BLOB Storing System.");
            }

            foreach (var provider in BlobProviders)
            {
                if (ProxyHelper.UnProxy(provider).GetType().IsAssignableTo(configuration.ProviderType))
                {
                    return provider;
                }
            }

            throw new AbpException(
                $"Could not find the BLOB Storage provider with the type ({configuration.ProviderType.AssemblyQualifiedName}) configured for the container {containerName} and no default provider was set."
            );
        }
    }
}
