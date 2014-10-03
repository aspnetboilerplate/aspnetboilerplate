using System.Data.Entity.Infrastructure.Interception;
using System.Reflection;
using Abp.EntityFramework.SoftDeleting;
using Abp.Modules;

namespace Abp.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            DbInterception.Add(new SoftDeleteInterceptor());
        }
    }
}
