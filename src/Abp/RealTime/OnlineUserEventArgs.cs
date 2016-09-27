namespace Abp.RealTime
{
    public class OnlineUserEventArgs : OnlineClientEventArgs
    {
        public UserIdentifier User { get; }

        public OnlineUserEventArgs(UserIdentifier user,IOnlineClient client) 
            : base(client)
        {
            User = user;
        }
    }
}