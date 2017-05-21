using System;

using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Abp.Orm
{
    public interface IAdditionalOrmRegistrar
    {
        string OrmContextKey { get; }

        event EventHandler RegistrationCallback;

        void RegisterRepositories(IIocManager iocManager, AutoRepositoryTypesAttribute defaultRepositoryTypes);
    }
}
