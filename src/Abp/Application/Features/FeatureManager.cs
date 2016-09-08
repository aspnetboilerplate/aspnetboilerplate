using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;

namespace Abp.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureManager"/>.
    /// </summary>
    internal class FeatureManager : FeatureDefinitionContextBase, IFeatureManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly IFeatureConfiguration _featureConfiguration;

        public FeatureManager(IIocManager iocManager, IFeatureConfiguration featureConfiguration)
        {
            _iocManager = iocManager;
            _featureConfiguration = featureConfiguration;
        }

        public void Initialize()
        {
            foreach (var providerType in _featureConfiguration.Providers)
            {
                using (var provider = CreateProvider(providerType))
                {
                    provider.Object.SetFeatures(this);
                }
            }

            Features.AddAllFeatures();
        }

        public Feature Get(string name)
        {
            var feature = GetOrNull(name);
            if (feature == null)
            {
                throw new AbpException("There is no feature with name: " + name);
            }

            return feature;
        }

        public IReadOnlyList<Feature> GetAll()
        {
            return Features.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<FeatureProvider> CreateProvider(Type providerType)
        {
            _iocManager.RegisterIfNot(providerType); //TODO: Needed?
            return _iocManager.ResolveAsDisposable<FeatureProvider>(providerType);
        }
    }
}
