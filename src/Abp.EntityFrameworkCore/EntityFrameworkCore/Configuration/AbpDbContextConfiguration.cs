using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Configuration;

public class AbpDbContextConfiguration<TDbContext>
    where TDbContext : DbContext
{
    public string ConnectionString { get; internal set; }

    public DbConnection ExistingConnection { get; internal set; }

    public DbContextOptionsBuilder<TDbContext> DbContextOptions { get; }

    public AbpDbContextConfiguration(string connectionString, DbConnection existingConnection)
    {
        ConnectionString = connectionString;
        ExistingConnection = existingConnection;

        DbContextOptions = new DbContextOptionsBuilder<TDbContext>();
    }
}