namespace Abp.Application.Session
{
    /// <summary>
    /// 
    /// </summary>
    public class NullAbpSession : IAbpSession
    {
        public static NullAbpSession Instance { get { return SingletonInstance; } }
        private static readonly NullAbpSession SingletonInstance = new NullAbpSession();
        public int? UserId { get { return null; } }
        public int? TenantId { get { return null; } }

        private NullAbpSession()
        {

        }
    }
}