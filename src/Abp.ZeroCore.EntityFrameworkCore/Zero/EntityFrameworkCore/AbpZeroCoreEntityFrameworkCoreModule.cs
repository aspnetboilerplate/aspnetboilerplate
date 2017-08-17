using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace Abp.Zero.EntityFrameworkCore
{
    /// <summary>
    /// Entity framework integration module for ASP.NET Boilerplate Zero.
    /// </summary>
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpEntityFrameworkCoreModule))]
    public class AbpZeroCoreEntityFrameworkCoreModule : AbpModule
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
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroCoreEntityFrameworkCoreModule).GetAssembly());
        }
    }
}
