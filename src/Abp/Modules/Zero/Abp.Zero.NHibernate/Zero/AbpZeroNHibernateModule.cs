using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Startup.Infrastructure.NHibernate;

namespace Abp.Zero
{
    public class AbpZeroNHibernateModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof(AbpZeroModule),
                       typeof (AbpNHibernateModule)
                   };
        }

        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);

            initializationContext.GetModule<AbpNHibernateModule>().Configuration
                .Mappings(
                    m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())
                );
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
