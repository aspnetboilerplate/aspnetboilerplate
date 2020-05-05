using Abp.Dependency;
using Abp.Threading;
using System.Threading;
using System.Web;

namespace Abp.Web.Mvc.Threading
{
    public class HttpContextCancellationTokenProvider : ICancellationTokenProvider, ITransientDependency
    {
        public CancellationToken Token => HttpContext.Current?.Response.ClientDisconnectedToken ?? CancellationToken.None;
    }
}
