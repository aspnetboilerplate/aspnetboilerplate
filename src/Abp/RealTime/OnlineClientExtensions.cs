using JetBrains.Annotations;

namespace Abp.RealTime
{
    public static class OnlineClientExtensions
    {
        [CanBeNull]
        public static UserIdentifier ToUserIdentifierOrNull(this IOnlineClient onlineClient)
        {
            if (onlineClient == null)
            {
                return null;
            }
            
            return onlineClient.UserId.HasValue
                ? new UserIdentifier(onlineClient.TenantId, onlineClient.UserId.Value)
                : null;
        }
    }
}