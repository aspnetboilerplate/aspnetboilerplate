using System;

using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Orm;

namespace Abp.NHibernate
{
    public class NhBasedAdditionalOrmRegistrar : IAdditionalOrmRegistrar, ITransientDependency
    {
        public string OrmContextKey => AbpConsts.Orms.NHibernate;

        public void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes)
        {
            iocManager.Register(defaultRepositoryTypes.RepositoryInterface, defaultRepositoryTypes.RepositoryImplementation);
            iocManager.Register(defaultRepositoryTypes.RepositoryInterfaceWithPrimaryKey, defaultRepositoryTypes.RepositoryImplementationWithPrimaryKey);
        }
    }
}
