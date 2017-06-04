#if NET46
using System;

using Abp.EntityFramework;

namespace Abp.EntityFrameworkCore
{
    public class EfCoreBasedSecondaryOrmRegistrar : SecondayOrmRegistrarBase
    {
        public EfCoreBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey { get; } = AbpConsts.Orms.EntityFrameworkCore;
    }
}
#endif
