using Abp.DapperCore.Repositories;

namespace Abp.DapperCore
{
    public static class EfBasedDapperAutoRepositoryTypes
    {
        static EfBasedDapperAutoRepositoryTypes()
        {
            Default = new DapperCoreAutoRepositoryTypeAttribute(
                typeof(IDapperCoreRepository<>),
                typeof(IDapperCoreRepository<,>),
                typeof(DapperEfCoreRepositoryBase<,>),
                typeof(DapperEfCoreRepositoryBase<,,>)
            );
        }

        public static DapperCoreAutoRepositoryTypeAttribute Default { get; }
    }
}
