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

        /// <summary>
        /// Creates a new <see cref="FeatureManager"/> object
        /// </summary>
        /// <param name="iocManager">IOC Manager</param>
        /// <param name="featureConfiguration">Feature configuration</param>
        public FeatureManager(IIocManager iocManager, IFeatureConfiguration featureConfiguration)
        {
            _iocManager = iocManager;
            _featureConfiguration = featureConfiguration;
        }

        /// <summary>
        /// Initializes this <see cref="FeatureManager"/>
        /// </summary>
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

        /// <summary>
        /// Gets a feature by its given name
        /// </summary>
        /// <param name="name">Name of the feature</param>
        public Feature Get(string name)
        {
            var feature = GetOrNull(name);
            if (feature == null)
            {
                throw new AbpException("There is no feature with name: " + name);
            }

            return feature;
        }

        /// <summary>
        /// Gets all the features
        /// </summary>
        public IReadOnlyList<Feature> GetAll()
        {
            return Features.Values.ToImmutableList();
        }

        private IDisposableDependencyObjectWrapper<FeatureProvider> CreateProvider(Type providerType)
        {
            return _iocManager.ResolveAsDisposable<FeatureProvider>(providerType);
        }
    }
}
