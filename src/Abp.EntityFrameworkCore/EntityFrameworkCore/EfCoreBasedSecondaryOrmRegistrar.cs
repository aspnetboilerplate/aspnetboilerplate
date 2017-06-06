using System;

using Abp.EntityFramework;

namespace Abp.EntityFrameworkCore
{
    public class EfCoreBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfCoreBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey { get; } = AbpConsts.Orms.EntityFrameworkCore;
    }
}
