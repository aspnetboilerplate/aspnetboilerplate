using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup.Infrastructure.EntityFramework
{
    /// <summary>
    /// This module is used to implement "Data Access Layer" in NHibernate.
    /// </summary>
    public class AbpEntityFrameworkModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
