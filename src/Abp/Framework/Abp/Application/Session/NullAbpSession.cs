namespace Abp.Application.Session
{
    /// <summary>
    /// Implements null object pattern for <see cref="IAbpSession"/>.
    /// </summary>
    public class NullAbpSession : IAbpSession
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullAbpSession Instance { get { return SingletonInstance; } }
        private static readonly NullAbpSession SingletonInstance = new NullAbpSession();
        
        public int? UserId { get { return null; } }
        
        public int? TenantId { get { return null; } }

        private NullAbpSession()
        {

        }
    }
}