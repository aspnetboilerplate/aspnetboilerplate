using Abp.Dependency;
using Microsoft.AspNet.SignalR.Hubs;

namespace Abp.RealTime
{
    public interface IOnlineClientInfoProvider : ITransientDependency
    {
        IOnlineClient CreateClientForCurrentConnection(HubCallerContext context);
    }
}
