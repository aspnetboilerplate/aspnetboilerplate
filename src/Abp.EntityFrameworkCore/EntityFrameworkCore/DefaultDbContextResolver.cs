using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Reflection;

namespace Abp.EntityFrameworkCore
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private static readonly MethodInfo CreateOptionsMethod = typeof(DefaultDbContextResolver).GetMethod("CreateOptions", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo CreateOptionsFromConnectionMethod = typeof(DefaultDbContextResolver).GetMethod("CreateOptionsFromConnection", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IIocResolver _iocResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        public DefaultDbContextResolver(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString)
            where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);

            if (!dbContextType.IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptions<TDbContext>(connectionString)
                });
            }

            var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);

            return (TDbContext)_iocResolver.Resolve(concreteType, new
            {
                options = CreateOptionsForType(concreteType, connectionString)
            });
        }

        public TDbContext Resolve<TDbContext>(DbConnection existingConnection) 
            where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);

            if (!dbContextType.IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptionsFromConnection<TDbContext>(existingConnection)
                });
            }

            var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);

            return (TDbContext)_iocResolver.Resolve(concreteType, new
            {
                options = CreateOptionsForType(concreteType, existingConnection)
            });
        }

        private object CreateOptionsForType(Type dbContextType, string connectionString)
        {
            return CreateOptionsMethod.MakeGenericMethod(dbContextType).Invoke(this, new object[] { connectionString });
        }

        private object CreateOptionsForType(Type dbContextType, DbConnection connection)
        {
            return CreateOptionsFromConnectionMethod.MakeGenericMethod(dbContextType).Invoke(this, new object[] { connection });
        }

        protected virtual DbContextOptions<TDbContext> CreateOptions<TDbContext>(string connectionString)
            where TDbContext : DbContext
        {
            if (_iocResolver.IsRegistered<IAbpDbContextConfigurer<TDbContext>>())
            {
                var configuration = new AbpDbContextConfiguration<TDbContext>(connectionString);

                using (var configurer = _iocResolver.ResolveAsDisposable<IAbpDbContextConfigurer<TDbContext>>())
                {
                    configurer.Object.Configure(configuration);
                }

                return configuration.DbContextOptions.Options;
            }

            if (_iocResolver.IsRegistered<DbContextOptions<TDbContext>>())
            {
                return _iocResolver.Resolve<DbContextOptions<TDbContext>>();
            }

            throw new AbpException($"Could not resolve DbContextOptions for {typeof(TDbContext).AssemblyQualifiedName}.");
        }

        protected virtual DbContextOptions<TDbContext> CreateOptionsFromConnection<TDbContext>(DbConnection existingConnection)
            where TDbContext : DbContext
        {
            if (_iocResolver.IsRegistered<IAbpDbContextConfigurer<TDbContext>>())
            {
                var configuration = new AbpDbContextConfiguration<TDbContext>(existingConnection);

                using (var configurer = _iocResolver.ResolveAsDisposable<IAbpDbContextConfigurer<TDbContext>>())
                {
                    configurer.Object.Configure(configuration);
                }

                return configuration.DbContextOptions.Options;
            }

            if (_iocResolver.IsRegistered<DbContextOptions<TDbContext>>())
            {
                return _iocResolver.Resolve<DbContextOptions<TDbContext>>();
            }

            throw new AbpException($"Could not resolve DbContextOptions for {typeof(TDbContext).AssemblyQualifiedName}.");
        }
    }
}