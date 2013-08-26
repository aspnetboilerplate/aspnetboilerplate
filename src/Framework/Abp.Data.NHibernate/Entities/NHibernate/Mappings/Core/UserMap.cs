using Abp.Entities.Core;
using FluentNHibernate.Mapping;

namespace Abp.Entities.NHibernate.Mappings.Core
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
            : base("Users")
        {
            Map(x => x.EmailAddress);
            Map(x => x.Password);
            HasMany(x => x.Accounts).Inverse().Cascade.All();
        }
    }
}
