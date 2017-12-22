using System.Reflection;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.MultiTenancy;
using Castle.MicroKernel.Registration;

namespace Abp.Zero.EntityFrameworkCore
{
    /// <summary>
    /// Entity framework integration module for ASP.NET Boilerplate Zero.
    /// </summary>
    [DependsOn(typeof(AbpZeroCoreModule), typeof(AbpEntityFrameworkCoreModule))]
    public class AbpZeroEntityFrameworkCoreModule : AbpModule
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

            AddIgnoredTypes();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        private void AddIgnoredTypes()
        {
            var ignoredTypes = new[]
            {
                typeof(AuditLog)
            };

            foreach (var ignoredType in ignoredTypes)
            {
                Configuration.EntityHistory.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }
    }
}
