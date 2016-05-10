using System;
using Abp.MultiTenancy;

namespace Abp.Runtime.Session
{
    /// <summary>
    ///     Implements null object pattern for <see cref="IAbpSession" />.
    /// </summary>
    public class NullAbpSession : IAbpSession
    {
        private NullAbpSession()
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullAbpSession Instance { get; } = new NullAbpSession();

        /// <inheritdoc />
        public Guid? UserId
        {
            get { return null; }
        }

        /// <inheritdoc />
        public Guid? TenantId
        {
            get { return null; }
        }

        public MultiTenancySides MultiTenancySide
        {
            get { return MultiTenancySides.Tenant; }
        }

        public Guid? ImpersonatorUserId
        {
            get { return null; }
        }

        public Guid? ImpersonatorTenantId
        {
            get { return null; }
        }
    }
}