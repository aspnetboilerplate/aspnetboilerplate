using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Application.Features
{
    public class FeatureChecker : IFeatureChecker, ITransientDependency
    {
        public IAbpSession AbpSession { get; set; }

        public IFeatureValueStore FeatureValueStore { get; set; }

        private readonly IFeatureManager _featureManager;

        public FeatureChecker(IFeatureManager featureManager)
        {
            _featureManager = featureManager;

            FeatureValueStore = NullFeatureValueStore.Instance;
            AbpSession = NullAbpSession.Instance;
        }

        public Task<string> GetValueAsync(string name)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new AbpException("FeatureChecker can not get a feature value by name since TenantId is not known!");
            }

            return GetValueAsync(AbpSession.TenantId.Value, name);
        }

        public async Task<string> GetValueAsync(int tenantId, string name)
        {
            var feature = _featureManager.Get(name);

            var value = await FeatureValueStore.GetValueOrNullAsync(tenantId, feature);
            if (value == null)
            {
                return feature.DefaultValue;
            }

            return value;
        }
    }
}