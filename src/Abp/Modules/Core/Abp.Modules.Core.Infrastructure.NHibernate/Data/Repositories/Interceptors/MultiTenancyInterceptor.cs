using System.Linq;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Security.Tenants;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Castle.DynamicProxy;

namespace Abp.Modules.Core.Data.Repositories.Interceptors
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    public class MultiTenancyInterceptor<TEntity, TPrimaryKey> : IInterceptor where TEntity : IEntity<TPrimaryKey>, IHasTenant
    {
        private readonly ITenantRepository _tenantRepository;

        public MultiTenancyInterceptor(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public void Intercept(IInvocation invocation)
        {
            //TODO: Implement better...

            if (invocation.MethodInvocationTarget.Name == "GetAll" && invocation.Arguments.IsNullOrEmpty())
            {
                invocation.Proceed();
                var returnValue = (IQueryable<TEntity>)invocation.ReturnValue;
                invocation.ReturnValue = returnValue.Where(entity => entity.Tenant.Id == Tenant.CurrentTenantId);
            }
            else if (invocation.MethodInvocationTarget.Name == "Insert")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is IHasTenant && invocation.Arguments[0].As<IHasTenant>().Tenant == null)
                    {
                        invocation.Arguments[0].As<IHasTenant>().Tenant = _tenantRepository.Load(Tenant.CurrentTenantId);
                    }
                }

                invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
