using Abp.Dependency;
using Abp.Threading;
using System.Threading;
using System.Web;
using Abp.Runtime;

namespace Abp.Web.Mvc.Threading
{
    public class HttpContextCancellationTokenProvider : CancellationTokenProviderBase, ITransientDependency
    {
        public override CancellationToken Token
        {
            get
            {
                if (OverridedValue != null)
                {
                    return OverridedValue.CancellationToken;
                }
                return HttpContext.Current?.Response.ClientDisconnectedToken ?? CancellationToken.None;
            }
        }
        public HttpContextCancellationTokenProvider(
            IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
            : base(cancellationTokenOverrideScopeProvider)
        {
        }
    }
}
