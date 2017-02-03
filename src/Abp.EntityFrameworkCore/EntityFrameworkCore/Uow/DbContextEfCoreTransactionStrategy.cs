using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Uow
{
  public class DbContextEfCoreTransactionStrategy : IEfCoreTransactionStrategy
  {
    public void Commit()
    {
      throw new NotImplementedException();
    }

    public DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver) where TDbContext : DbContext
    {
      throw new NotImplementedException();
    }

    public void Dispose(IIocResolver iocResolver)
    {
      throw new NotImplementedException();
    }

    public void InitOptions(UnitOfWorkOptions options)
    {
      throw new NotImplementedException();
    }
  }
}
