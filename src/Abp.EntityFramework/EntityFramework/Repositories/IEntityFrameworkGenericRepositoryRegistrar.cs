using System;
using Abp.Dependency;

namespace Abp.EntityFramework.Repositories
{
    public interface IEntityFrameworkGenericRepositoryRegistrar
    {
        void RegisterForDbContext(Type dbContextType, IIocManager iocManager);
    }
}