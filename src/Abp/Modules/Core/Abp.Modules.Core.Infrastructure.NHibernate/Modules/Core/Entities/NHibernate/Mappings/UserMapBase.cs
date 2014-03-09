using Abp.Domain.Entities.Mapping;
using Abp.Security.Users;

namespace Abp.Modules.Core.Entities.NHibernate.Mappings
{
    public abstract class UserMapBase<TUser> : EntityMap<TUser> where TUser : AbpUser
    {
        protected UserMapBase()
            : base("AbpUsers")
        {
            Map(x => x.UserName);
            Map(x => x.Name);
            Map(x => x.Surname);
            Map(x => x.EmailAddress);
            Map(x => x.IsEmailConfirmed);
            Map(x => x.EmailConfirmationCode);
            Map(x => x.Password);
            Map(x => x.PasswordResetCode);
        }
    }
}
