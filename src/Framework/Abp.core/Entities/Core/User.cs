using System;
using System.Collections.Generic;

namespace Abp.Entities.Core
{
    /// <summary>
    /// Represents a user in entire system.
    /// </summary>
    public class User : Entity
    {
        /// <summary>
        /// Name of the user.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public virtual string Surname { get; set; }

        /// <summary>
        /// Email address of the user.
        /// </summary>
        public virtual string EmailAddress { get; set; }

        /// <summary>
        /// Password of the user.
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// List of all tenant accounts of this user.
        /// </summary>
        public virtual IList<Tenant> Tenancies { get; set; }

        [ThreadStatic]
        private static User _current;
        public static User Current
        {
            get { return _current ?? new User { Id = 1 }; } //TODO: Remove dummy entity
            set { _current = value; }
        }
    }
}