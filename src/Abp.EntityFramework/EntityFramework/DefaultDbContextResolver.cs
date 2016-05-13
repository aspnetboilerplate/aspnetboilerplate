using Abp.Dependency;

namespace Abp.EntityFramework
{
    public class DefaultDbContextResolver : IDbContextResolver, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IDbContextTypeMatcher _dbContextTypeMatcher;

        public DefaultDbContextResolver(IIocResolver iocResolver, IDbContextTypeMatcher dbContextTypeMatcher)
        {
            _iocResolver = iocResolver;
            _dbContextTypeMatcher = dbContextTypeMatcher;
        }

        public TDbContext Resolve<TDbContext>(string connectionString)
        {
            var dbContextType = typeof(TDbContext);

            if (!dbContextType.IsAbstract)
            {
                return _iocResolver.Resolve<TDbContext>(new
                {
                    nameOrConnectionString = connectionString
                });
            }
            else
            {
                var concreteType = _dbContextTypeMatcher.GetConcreteType(dbContextType);
                return (TDbContext)_iocResolver.Resolve(concreteType, new
                {
                    nameOrConnectionString = connectionString
                });
            }
        }
    }
}