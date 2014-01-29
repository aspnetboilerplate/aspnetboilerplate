using Abp.Domain.Entities;

namespace Abp.Security.Users
{
    /// <summary>
    /// TODO: Add a unique index for UserId, RoleId
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class UserLogin<TUser> : Entity<long> where TUser : AbpUser
    {
        public virtual TUser User { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
