using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace Abp.Zero.EntityFramework
{
    /// <summary>
    /// Entity framework integration module for ASP.NET Boilerplate Zero.
    /// </summary>
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpEntityFrameworkModule))]
    public class AbpZeroCoreEntityFrameworkModule : AbpModule
    {
        public override void PreInitialize()
        {
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
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreEntityFrameworkModule).GetAssembly());
        }
    }
}
