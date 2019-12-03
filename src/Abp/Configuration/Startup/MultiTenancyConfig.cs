using Abp.Collections;
using Abp.MultiTenancy;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used to configure multi-tenancy.
    /// </summary>
    internal class MultiTenancyConfig : IMultiTenancyConfig
    {
        /// <summary>
        /// Is multi-tenancy enabled?
        /// Default value: false.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Ignore feature check for host users
        /// Default value: false.
        /// </summary>
        public bool IgnoreFeatureCheckForHostUsers { get; set; }

        public ITypeList<ITenantResolveContributor> Resolvers { get; }

        public string TenantIdResolveKey { get; set; }

        public MultiTenancyConfig()
        {
            Resolvers = new TypeList<ITenantResolveContributor>();
            TenantIdResolveKey = "Abp.TenantId";
        }
    }
}