using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.NHibernate;
using Castle.MicroKernel.Registration;
using System.Reflection;

namespace Abp.Zero.NHibernate
{
    /// <summary>
    /// Startup class for ABP Zero NHibernate module.
    /// </summary>
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpNHibernateModule))]
    public class AbpZeroCoreNHibernateModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpNHibernate().FluentConfiguration
                .Mappings(
                    m => m.FluentMappings.AddFromAssemblyOf<AbpZeroCoreNHibernateModule>()
                );

            Configuration.ReplaceService(typeof(IConnectionStringResolver), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IConnectionStringResolver, IDbPerTenantConnectionStringResolver>()
                        .ImplementedBy<DbPerTenantConnectionStringResolver>()
                        .LifestyleTransient()
                );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
