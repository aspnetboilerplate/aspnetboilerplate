using Abp.Dependency;
using Abp.Modules;
using Abp.Orm;
using Abp.Reflection;
using Abp.Reflection.Extensions;

namespace Abp.DapperCore
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpDapperCoreModule : AbpModule
    {
        private readonly ITypeFinder _typeFinder;

        public AbpDapperCoreModule(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpDapperCoreModule).GetAssembly());

            using (var scope = IocManager.CreateScope())
            {
                var additionalOrmRegistrars = scope.ResolveAll<ISecondaryOrmRegistrar>();

                foreach (var registrar in additionalOrmRegistrars)
                {
                    if (registrar.OrmContextKey == AbpConsts.Orms.EntityFramework)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == AbpConsts.Orms.NHibernate)
                    {
                        registrar.RegisterRepositories(IocManager, NhBasedDapperAutoRepositoryTypes.Default);
                    }
                }
            }
        }
    }
}
