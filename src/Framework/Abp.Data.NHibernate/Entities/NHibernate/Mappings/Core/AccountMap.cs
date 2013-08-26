using Abp.Entities.Core;

namespace Abp.Entities.NHibernate.Mappings.Core
{
    public class AccountMap : EntityMap<Account, int>
    {
        public AccountMap()
            : base("Accounts")
        {
            Map(x => x.CompanyName);
            References(x => x.Owner).Column("OwnerUserId");
        }
    }
}