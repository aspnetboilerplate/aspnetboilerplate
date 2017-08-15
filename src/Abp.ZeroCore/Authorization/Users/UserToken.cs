using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using JetBrains.Annotations;

namespace Abp.Authorization.Users
{
    /// <summary>
    /// Represents an authentication token for a user.
    /// </summary>
    [Table("AbpUserTokens")]
    public class UserToken : Entity<long>, IMayHaveTenant
    {
        public const int MaxLoginProviderLength = 64;

        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Gets or sets the primary key of the user that the token belongs to.
        /// </summary>
        public virtual long UserId { get; set; }

        /// <summary>
        /// Gets or sets the LoginProvider this token is from.
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// Gets or sets the name of the token.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the token value.
        /// </summary>
        public virtual string Value { get; set; }

        protected UserToken()
        {
            
        }

        protected internal UserToken(AbpUserBase user, [NotNull] string loginProvider, [NotNull] string name, string value)
        {
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(name, nameof(name));

            TenantId = user.TenantId;
            UserId = user.Id;
            LoginProvider = loginProvider;
            Name = name;
            Value = value;
        }
    }
}