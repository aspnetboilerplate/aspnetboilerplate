using System.Linq;
using Abp.Domain.Entities;
using Abp.Tenants;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Castle.DynamicProxy;
using NHibernate.Linq;

namespace Abp.Modules.Core.Data.Repositories.Interceptors
{
    /// <summary>
    /// TODO: Don't forget! Must intercept classes, not interfaces to be able to intercept virtual methods not called from outside of the class!
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
            if (invocation.MethodInvocationTarget.Name == "GetAll" && invocation.Arguments.IsNullOrEmpty())
            {
                invocation.Proceed();
                var returnValue = (IQueryable<TEntity>)invocation.ReturnValue;
                invocation.ReturnValue = returnValue.Where(entity => entity.Tenant.Id == 1); //TODO: Set Real Tenant ID
            }
            else if (invocation.MethodInvocationTarget.Name == "Insert")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is IHasTenant && invocation.Arguments[0].As<IHasTenant>().Tenant == null)
                    {
                        invocation.Arguments[0].As<IHasTenant>().Tenant = _tenantRepository.Load(1);  //TODO: Get Real Tenant                            
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
