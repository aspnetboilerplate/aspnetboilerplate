using Abp.Domain.Entities;

namespace Abp.Security.Users
{
    public class UserLogin<TUser> : Entity<long> where TUser : User
    {
        public virtual TUser User { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
