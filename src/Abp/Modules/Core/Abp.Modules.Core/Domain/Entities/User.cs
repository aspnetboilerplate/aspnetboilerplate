using System;
using System.Collections.Generic;
using System.Threading;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities.Utils;
using Abp.Security;

namespace Abp.Modules.Core.Domain.Entities
{
    /// <summary>
    /// Represents a user in entire system.
    /// </summary>
    public class User : Entity, IHasTenant
    {
        /// <summary>
        /// Tenant of this role.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

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
        /// Profile image of the user. 
        /// </summary>
        public virtual string ProfileImage { get; set; }

        /// <summary>
        /// Is this user owner of the <see cref="Tenant"/>.
        /// </summary>
        public virtual bool IsTenantOwner { get; set; }

        /// <summary>
        /// Gets Name and Surname joined by space.
        /// </summary>
        public virtual string NameAndSurname { get { return Name + " " + Surname; } }

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