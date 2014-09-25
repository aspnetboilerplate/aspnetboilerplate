using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using Abp.Dependency;
using Abp.Domain.Repositories.EntityFramework.SoftDeleting;
using Abp.Modules;

namespace Abp.Startup.Infrastructure.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext context)
        {
            base.Initialize(context);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DbInterception.Add(new SoftDeleteInterceptor());
        }
    }
}
