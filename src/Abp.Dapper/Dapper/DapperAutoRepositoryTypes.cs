using Abp.Dapper.Repositories;
using JetBrains.Annotations;

namespace Abp.Dapper
{
    public static class DapperAutoRepositoryTypes
    {
        static DapperAutoRepositoryTypes()
        {
            Default = new DapperAutoRepositoryTypeAttribute(
                typeof(IDapperRepository<>),
                typeof(IDapperRepository<,>),
                typeof(DapperRepositoryBase<,>),
                typeof(DapperRepositoryBase<,,>)
            );
        }

        [NotNull]
        public static DapperAutoRepositoryTypeAttribute Default { get; private set; }
    }
}
