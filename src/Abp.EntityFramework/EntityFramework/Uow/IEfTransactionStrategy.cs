using System;
using System.Data.Entity;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Uow
{
    public interface IEfTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        void Commit();

        void InitDbContext(DbContext dbContext, string connectionString);

        void Dispose(IIocResolver iocResolver);
    }
}