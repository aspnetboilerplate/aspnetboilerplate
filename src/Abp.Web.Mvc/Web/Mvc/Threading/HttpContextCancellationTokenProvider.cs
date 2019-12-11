using Abp.Dependency;
using Abp.Threading;
using System.Threading;
using System.Web;

namespace Abp.Web.Mvc.Threading
{
    public class HttpContextCancellationTokenProvider : ICancellationTokenProvider, ITransientDependency
    {
        public CancellationToken Token => CancellationTokenSource.CreateLinkedTokenSource(default(CancellationToken), HttpContext.Current.Response.ClientDisconnectedToken)?.Token ?? CancellationToken.None;
    }
}
