using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Tests.Ef
{
    [AutoRepositoryTypes(
        typeof(IShopRepository<>),
        typeof(IShopRepository<,>),
        typeof(ShopRepositoryBase<>),
        typeof(ShopRepositoryBase<,>),
        WithDefaultRepositoryInterfaces = true
    )]
    public class ShopDbContext : AbpDbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<ProductTranslation> ProductTranslations { get; set; }

        public ShopDbContext(DbContextOptions<ShopDbContext> options)
            : base(options)
        {

        }
    }

    public interface IShopRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        //A new custom method
        List<TEntity> GetActiveList();
    }

    public interface IShopRepository<TEntity> : IShopRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {

    }

    public class ShopRepositoryBase<TEntity, TPrimaryKey> : EfCoreRepositoryBase<ShopDbContext, TEntity, TPrimaryKey>, IShopRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public ShopRepositoryBase(IDbContextProvider<ShopDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //A new custom method
        public List<TEntity> GetActiveList()
        {
            if (typeof(IPassivable).GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
            {
                return GetAll()
                    .Cast<IPassivable>()
                    .Where(e => e.IsActive)
                    .Cast<TEntity>()
                    .ToList();
            }

            return GetAllList();
        }

        //An override of a default method
        public override int Count()
        {
            throw new Exception("can not get count!");
        }
    }

    public class ShopRepositoryBase<TEntity> : ShopRepositoryBase<TEntity, int>, IShopRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public ShopRepositoryBase(IDbContextProvider<ShopDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }
    }
}