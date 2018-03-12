using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace Abp.EntityFrameworkCore
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private static readonly MethodInfo CreateOptionsMethod = typeof(DefaultDbContextResolver).GetMethod("CreateOptions", BindingFlags.NonPublic | BindingFlags.Instance);

        private readonly IIocResolver _iocResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        public DefaultDbContextResolver(
            IIocResolver iocResolver,
            IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString, DbConnection existingConnection)
            where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);
            Type concreteType = null;
            var isAbstractDbContext = dbContextType.GetTypeInfo().IsAbstract;
            if (isAbstractDbContext)
            {
                concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);
            }

            try
            {
                if (isAbstractDbContext)
                {
                    return (TDbContext) _iocResolver.Resolve(concreteType, new
                    {
                        options = CreateOptionsForType(concreteType, connectionString, existingConnection)
                    });
                }

                return _iocResolver.Resolve<TDbContext>(new
                {
                    options = CreateOptions<TDbContext>(connectionString, existingConnection)
                });
            }
            catch (Castle.MicroKernel.Resolvers.DependencyResolverException ex)
            {
                var hasOptions = isAbstractDbContext ? HasOptions(concreteType) : HasOptions(dbContextType);
                if (!hasOptions)
                {
                    throw new AggregateException($"The parameter name of {dbContextType.Name}'s constructor must be 'options'", ex);
                }

                throw;
            }

            bool HasOptions(Type contextType)
            {
                return contextType.GetConstructors().Any(ctor =>
                {
                    var parameters = ctor.GetParameters();
                    return parameters.Length == 1 && parameters.FirstOrDefault()?.Name == "options";
                });
            }
        }

        private object CreateOptionsForType(Type dbContextType, string connectionString, DbConnection existingConnection)
        {
            return CreateOptionsMethod.MakeGenericMethod(dbContextType).Invoke(this, new object[] { connectionString, existingConnection });
        }

        protected virtual DbContextOptions<TDbContext> CreateOptions<TDbContext>([NotNull] string connectionString, [CanBeNull] DbConnection existingConnection) where TDbContext : DbContext
        {
            if (_iocResolver.IsRegistered<IAbpDbContextConfigurer<TDbContext>>())
            {
                var configuration = new AbpDbContextConfiguration<TDbContext>(connectionString, existingConnection);
                ReplaceServices(configuration);

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

        protected virtual void ReplaceServices<TDbContext>(AbpDbContextConfiguration<TDbContext> configuration) where TDbContext : DbContext
        {
            configuration.DbContextOptions.ReplaceService<IEntityMaterializerSource, AbpEntityMaterializerSource>();
        }
    }
}