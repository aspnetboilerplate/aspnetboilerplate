using Abp.Dependency;

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
        {
            //TODO: connectionString is not used. We should find a way of creating DbContextOptions<TDbContext> based on that connection string

            var dbContextType = typeof(TDbContext);

            if (!dbContextType.IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>();
            }

            var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);
            return (TDbContext)_iocResolver.Resolve(concreteType);
        }
    }
}