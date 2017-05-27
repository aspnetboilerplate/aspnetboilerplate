using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Orm;
using Castle.Core.Internal;

namespace Abp.Dapper
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpDapperModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactionScopeAvailable = false;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            using (var scope = IocManager.CreateScope())
            {
                var additionalOrmRegistrars = scope.ResolveAll<IAdditionalOrmRegistrar>();

                additionalOrmRegistrars.ForEach(registrar =>
                {
                    if (registrar.OrmContextKey == AbpConsts.Orms.EntityFramework)
                    {
                        registrar.RegisterRepositories(IocManager, EfBasedDapperAutoRepositoryTypes.Default);
                    }

                    if (registrar.OrmContextKey == AbpConsts.Orms.NHibernate)
                    {
                        registrar.RegisterRepositories(IocManager, NhBasedDapperAutoRepositoryTypes.Default);
                    }
                });
            }
        }
    }
}
