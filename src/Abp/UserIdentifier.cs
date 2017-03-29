using System;
using System.Reflection;
using Abp.Extensions;

namespace Abp
{
    /// <summary>
    /// Used to identify a user.
    /// </summary>
    [Serializable]
    public class UserIdentifier : IUserIdentifier
    {
        /// <summary>
        /// Tenant Id of the user.
        /// Can be null for host users in a multi tenant application.
        /// </summary>
        public int? TenantId { get; protected set; }

        /// <summary>
        /// Id of the user.
        /// </summary>
        public long UserId { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// </summary>
        protected UserIdentifier()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// </summary>
        /// <param name="tenantId">Tenant Id of the user.</param>
        /// <param name="userId">Id of the user.</param>
        public UserIdentifier(int? tenantId, long userId)
        {
            TenantId = tenantId;
            UserId = userId;
        }

        /// <summary>
        /// Parses given string and creates a new <see cref="UserIdentifier"/> object.
        /// </summary>
        /// <param name="userIdentifierString">
        /// Should be formatted one of the followings:
        /// 
        /// - "userId@tenantId". Ex: "42@3" (for tenant users).
        /// - "userId". Ex: 1 (for host users)
        /// </param>
        public static UserIdentifier Parse(string userIdentifierString)
        {
            if (userIdentifierString.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userIdentifierString), "userAtTenant can not be null or empty!");
            }

            var splitted = userIdentifierString.Split('@');
            if (splitted.Length == 1)
            {
                return new UserIdentifier(null, splitted[0].To<long>());

            }

            if (splitted.Length == 2)
            {
                return new UserIdentifier(splitted[1].To<int>(), splitted[0].To<long>());
            }

            throw new ArgumentException("userAtTenant is not properly formatted", nameof(userIdentifierString));
        }

        /// <summary>
        /// Creates a string represents this <see cref="UserIdentifier"/> instance.
        /// Formatted one of the followings:
        /// 
        /// - "userId@tenantId". Ex: "42@3" (for tenant users).
        /// - "userId". Ex: 1 (for host users)
        /// 
        /// Returning string can be used in <see cref="Parse"/> method to re-create identical <see cref="UserIdentifier"/> object.
        /// </summary>
        public string ToUserIdentifierString()
        {
            if (TenantId == null)
            {
                return UserId.ToString();
            }

            return UserId + "@" + TenantId;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is UserIdentifier))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            var other = (UserIdentifier)obj;

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return TenantId == other.TenantId && UserId == other.UserId;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return TenantId == null ? (int)UserId : (int)(TenantId.Value ^ UserId);
        }

        /// <inheritdoc/>
        public static bool operator ==(UserIdentifier left, UserIdentifier right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(UserIdentifier left, UserIdentifier right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return ToUserIdentifierString();
        }
    }
}
