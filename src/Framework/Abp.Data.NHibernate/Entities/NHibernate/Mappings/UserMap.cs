using Abp.Entities.Core;
using FluentNHibernate.Mapping;

namespace Abp.Entities.NHibernate.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");

            Id(x => x.Id);

            Map(x => x.EmailAddress);
            Map(x => x.Password);

            HasMany(x => x.Accounts).Inverse().Cascade.All();
        }
    }
}
