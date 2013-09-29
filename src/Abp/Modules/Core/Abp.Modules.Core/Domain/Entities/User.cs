using System;
using System.Collections.Generic;
using System.Threading;
using Abp.Domain.Entities;
using Abp.Security;

namespace Abp.Modules.Core.Domain.Entities
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
        /// List of all owned tenant accounts of this user.
        /// </summary>
        public virtual IList<Tenant> Tenancies { get; set; }

        /// <summary>
        /// List of all tenant memberships of this user.
        /// </summary>
        public virtual IList<TenantUser> TenantMemberships { get; set; }

        /// <summary>
        /// Gets current user id.
        /// </summary>
        public static int CurrentUserId
        {
            get
            {
                if (Thread.CurrentPrincipal == null)
                {
                    throw new ApplicationException("Thread.CurrentPrincipal is null!");
                }

                var identity = Thread.CurrentPrincipal.Identity as AbpIdentity;
                if (identity == null)
                {
                    throw new ApplicationException("Thread.CurrentPrincipal.Identity is not type of AbpIdentity!");
                }

                return identity.UserId;
            }
        }
    }
}