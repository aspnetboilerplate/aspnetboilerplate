using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Orm;
using Castle.MicroKernel.Registration;

namespace Abp.NHibernate
{
    public class NhBasedSecondaryOrmRegistrar : ISecondaryOrmRegistrar, ITransientDependency
    {
        public string OrmContextKey => AbpConsts.Orms.NHibernate;

        public void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes)
        {
            if (defaultRepositoryTypes.WithDefaultRepositoryInterfaces)
            {
                iocManager.IocContainer.Register(
                    Component.For(typeof(IRepository<>),defaultRepositoryTypes.RepositoryInterface).ImplementedBy(defaultRepositoryTypes.RepositoryImplementation).LifestyleTransient(),
                    Component.For(typeof(IRepository<,>),defaultRepositoryTypes.RepositoryInterfaceWithPrimaryKey).ImplementedBy(defaultRepositoryTypes.RepositoryImplementationWithPrimaryKey).LifestyleTransient()
                );
            }
            else
            {
                iocManager.Register(defaultRepositoryTypes.RepositoryInterface, defaultRepositoryTypes.RepositoryImplementation, DependencyLifeStyle.Transient);
                iocManager.Register(defaultRepositoryTypes.RepositoryInterfaceWithPrimaryKey, defaultRepositoryTypes.RepositoryImplementationWithPrimaryKey, DependencyLifeStyle.Transient);
            }
        }
    }
}
