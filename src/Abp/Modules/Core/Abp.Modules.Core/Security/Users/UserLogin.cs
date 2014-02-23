using Abp.Domain.Entities;

namespace Abp.Security.Users
{
    /// <summary>
    /// TODO: Add a unique index for UserId, RoleId
    /// </summary>
    public class UserLogin : Entity<long>
    {
        public virtual int UserId { get; set; }

        public virtual string LoginProvider { get; set; }

        public virtual string ProviderKey { get; set; }
    }
}
