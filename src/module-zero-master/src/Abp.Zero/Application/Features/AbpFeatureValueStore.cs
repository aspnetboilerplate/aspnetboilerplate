using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.MultiTenancy;

namespace Abp.Application.Features
{
    /// <summary>
    /// Implements <see cref="IFeatureValueStore"/>.
    /// </summary>
    public abstract class AbpFeatureValueStore<TTenant, TRole, TUser> : IFeatureValueStore, ITransientDependency 
        where TTenant : AbpTenant<TTenant, TUser> 
        where TRole : AbpRole<TTenant, TUser> 
        where TUser : AbpUser<TTenant, TUser>
    {
        private readonly AbpTenantManager<TTenant, TRole, TUser> _tenantManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbpFeatureValueStore{TTenant, TRole, TUser}"/> class.
        /// </summary>
        /// <param name="tenantManager">The tenant manager.</param>
        protected AbpFeatureValueStore(AbpTenantManager<TTenant, TRole, TUser> tenantManager)
        {
            _tenantManager = tenantManager;
        }

        /// <inheritdoc/>
        public virtual Task<string> GetValueOrNullAsync(int tenantId, Feature feature)
        {
            return _tenantManager.GetFeatureValueOrNullAsync(tenantId, feature.Name);
        }
    }
}