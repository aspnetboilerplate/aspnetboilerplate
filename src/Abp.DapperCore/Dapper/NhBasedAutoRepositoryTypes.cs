using Abp.DapperCore.Repositories;

namespace Abp.DapperCore
{
    public class NhBasedDapperAutoRepositoryTypes
    {
        static NhBasedDapperAutoRepositoryTypes()
        {
            Default = new DapperCoreAutoRepositoryTypeAttribute(
                typeof(IDapperCoreRepository<>),
                typeof(IDapperCoreRepository<,>),
                typeof(DapperCoreRepositoryBase<>),
                typeof(DapperCoreRepositoryBase<,>)
            );
        }

        public static DapperCoreAutoRepositoryTypeAttribute Default { get; }
    }
}
