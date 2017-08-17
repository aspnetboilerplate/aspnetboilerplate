using System;
using Abp.Dependency;

namespace Abp.EntityFrameworkCore.Repositories
{
    public interface IEfCoreGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager);
    }
}