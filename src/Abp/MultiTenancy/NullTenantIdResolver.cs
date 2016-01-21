namespace Abp.MultiTenancy
{
    /// <summary>
    /// Implements null object pattern for <see cref="ITenantIdResolver"/>.
    /// </summary>
    public class NullTenantIdResolver : ITenantIdResolver
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullTenantIdResolver Instance { get { return SingletonInstance; } }
        private static readonly NullTenantIdResolver SingletonInstance = new NullTenantIdResolver();

        public int? TenantId { get { return null; } }
    }
}