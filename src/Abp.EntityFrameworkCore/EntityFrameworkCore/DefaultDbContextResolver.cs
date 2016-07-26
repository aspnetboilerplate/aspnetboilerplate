using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
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
            var dbContextConstructorArgs = new
            {
                options = CreateOptions<TDbContext>(connectionString)
            };

            var dbContextType = typeof(TDbContext);
            
            if (!dbContextType.IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>(dbContextConstructorArgs);
            }

            var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);

            return (TDbContext)_iocResolver.Resolve(concreteType, dbContextConstructorArgs);
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
    }
}