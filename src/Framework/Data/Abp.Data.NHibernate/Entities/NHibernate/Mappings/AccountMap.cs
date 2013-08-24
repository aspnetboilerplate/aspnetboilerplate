using Abp.Entities.Core;
using FluentNHibernate.Mapping;

namespace Abp.Entities.NHibernate.Mappings
{
    public class AccountMap : ClassMap<Account>
    {
        public AccountMap()
        {
            Table("Accounts");

            Id(x => x.Id);

            Map(x => x.CompanyName);
            References(x => x.Owner).Column("OwnerUserId");

            Map(x => x.CreationTime);
            References(x => x.Creator).Column("CreatorUserId");

            Map(x => x.LastModificationTime);
            References(x => x.LastModifier).Column("LastModifierUserId");
        }
    }
}