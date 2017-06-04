using System;

namespace Abp.EntityFramework
{
    public class EfBasedSecondaryOrmRegistrar : SecondayOrmRegistrarBase
    {
        public EfBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey => AbpConsts.Orms.EntityFramework;
    }
}
