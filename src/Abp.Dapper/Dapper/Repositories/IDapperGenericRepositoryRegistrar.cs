using System;
using Abp.Dependency;
using JetBrains.Annotations;

namespace Abp.Dapper.Repositories
{
    public interface IDapperGenericRepositoryRegistrar
    {
        void RegisterForDbContext([NotNull] Type dbContextType, [NotNull] IIocManager iocManager);
    }
}
