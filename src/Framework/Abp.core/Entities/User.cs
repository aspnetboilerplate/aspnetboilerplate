using System.Collections.Generic;

namespace Abp.Entities
{
    /// <summary>
    /// Represents a user in entire system.
    /// </summary>
    public class User : Entity<int>
    {
        /// <summary>
        /// Email address of the user.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// List of all accounts of this user.
        /// </summary>
        public virtual IList<Account> Accounts { get; set; }
    }
}