namespace Abp.RealTime
{
    public static class OnlineClientExtensions
    {
        public static UserIdentifier ToUserIdentifier(this IOnlineClient onlineClient)
        {
            return onlineClient.UserId.HasValue
                ? new UserIdentifier(onlineClient.TenantId, onlineClient.UserId.Value)
                : null;
        }
    }
}