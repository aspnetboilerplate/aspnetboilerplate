using System;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Application.Features
{
    /// <summary>
    /// Default implementation for <see cref="IFeatureChecker"/>.
    /// </summary>
    public class FeatureChecker : IFeatureChecker, ITransientDependency, IIocManagerAccessor
    {
        /// <summary>
        /// Reference to the current session.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Reference to the store used to get feature values.
        /// </summary>
        public IFeatureValueStore FeatureValueStore { get; set; }

        public IIocManager IocManager { get; set; }

        private readonly IFeatureManager _featureManager;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        /// <summary>
        /// Creates a new <see cref="FeatureChecker"/> object.
        /// </summary>
        public FeatureChecker(IFeatureManager featureManager, IMultiTenancyConfig multiTenancyConfig)
        {
            _featureManager = featureManager;
            _multiTenancyConfig = multiTenancyConfig;

            FeatureValueStore = NullFeatureValueStore.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        /// <inheritdoc/>
        public Task<string> GetValueAsync(string name)
        {
            if (AbpSession.TenantId == null)
            {
                throw new AbpException("FeatureChecker can not get a feature value by name. TenantId is not set in the IAbpSession!");
            }

            return GetValueAsync(AbpSession.TenantId.Value, name);
        }

        /// <inheritdoc/>
        public string GetValue(string name)
        {
            if (AbpSession.TenantId == null)
            {
                throw new AbpException("FeatureChecker can not get a feature value by name. TenantId is not set in the IAbpSession!");
            }

            return GetValue(AbpSession.TenantId.Value, name);
        }

        /// <inheritdoc/>
        public async Task<string> GetValueAsync(int tenantId, string name)
        {
            var feature = _featureManager.Get(name);
            var value = await FeatureValueStore.GetValueOrNullAsync(tenantId, feature);

            return value ?? feature.DefaultValue;
        }

        /// <inheritdoc/>
        public string GetValue(int tenantId, string name)
        {
            var feature = _featureManager.Get(name);
            var value = FeatureValueStore.GetValueOrNull(tenantId, feature);

            return value ?? feature.DefaultValue;
        }

        /// <inheritdoc/>
        public async Task<bool> IsEnabledAsync(string featureName)
        {
            if (AbpSession.TenantId == null && _multiTenancyConfig.IgnoreFeatureCheckForHostUsers)
            {
                return true;
            }

            return string.Equals(await GetValueAsync(featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool IsEnabled(string featureName)
        {
            if (AbpSession.TenantId == null && _multiTenancyConfig.IgnoreFeatureCheckForHostUsers)
            {
                return true;
            }

            return string.Equals(GetValue(featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public async Task<bool> IsEnabledAsync(int tenantId, string featureName)
        {
            return string.Equals(await GetValueAsync(tenantId, featureName), "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        public bool IsEnabled(int tenantId, string featureName)
        {
            return string.Equals(GetValue(tenantId, featureName), "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}