using Abp.Modules.Core.Domain.Entities;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public class UserMap : EntityMap<User>
    {
        public UserMap()
            : base("AbpUsers")
        {
            Map(x => x.Name);
            Map(x => x.Surname);
            Map(x => x.EmailAddress);
            Map(x => x.IsEmailConfirmed);
            Map(x => x.EmailConfirmationCode);
            Map(x => x.Password);
            Map(x => x.PasswordResetCode);
            Map(x => x.ProfileImage);
            Map(x => x.IsTenantOwner);
        }
    }
}
