using System;

namespace Abp.Entities.Core
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

        [ThreadStatic]
        private static Tenant _current;
        public static Tenant Current
        {
            get { return new Tenant { Id = 1 }; }
            set { _current = value; }
        }

        #endregion
    }
}
