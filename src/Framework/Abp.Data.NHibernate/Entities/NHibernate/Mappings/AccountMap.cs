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
            Map(x => x.Owner).Column("OwnerUserId"); //TODO: ???

            Map(x => x.CreationDate);
            Map(x => x.Creator).Column("CreatorUserId"); //TODO: ???
            Map(x => x.LastModificationDate);
            Map(x => x.LastModifier).Column("LastModifierUserId"); //TODO: ???
        }
    }
}