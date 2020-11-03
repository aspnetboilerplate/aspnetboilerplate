using Abp.Dependency;
using Microsoft.AspNetCore.SignalR;

namespace Abp.RealTime
{
    public interface IOnlineClientInfoProvider : ITransientDependency
    {
        IOnlineClient CreateClientForCurrentConnection(HubCallerContext context);
    }
}
