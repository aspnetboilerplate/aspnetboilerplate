using Abp.Domain.Entities;

namespace Abp.Security.Users
{
    public class UserLogin : Entity<long>
    {
        public virtual int UserId { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
