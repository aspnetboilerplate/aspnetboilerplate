using System;
using Abp.Modules.Core.Domain.Entities.Utils;

namespace Abp.Modules.Core.Domain.Entities
{
    /// <summary>
    /// Represents a tenant account. A tenant is used in the cloud to identify a seperated application in the system.
    /// </summary>
    public class Tenant : AuditedEntity
    {
        /// <summary>
        /// Company name
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Tenant owner.
        /// </summary>
        public virtual User Owner { get; set; }

        #region Static properties

        /// <summary>
        /// Reference to the current Tenant account of current user.
        /// </summary>
        [ThreadStatic]
        private static Tenant _current;
        public static Tenant Current
        {
            get { return _current ?? new Tenant { Id = 1 }; } //TODO: Remove dummy entity
            set { _current = value; }
        }

        #endregion
    }
}
