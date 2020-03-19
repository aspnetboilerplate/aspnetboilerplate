using Abp.Dependency;
using Abp.Threading;
using System.Threading;
using System.Web;
using Abp.Runtime;

namespace Abp.Web.Threading
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

                try
                {
                    return HttpContext.Current?.Response.ClientDisconnectedToken ?? CancellationToken.None;
                }
                catch (HttpException ex)
                {
                    /* Workaround:
                     * Accessing HttpContext.Response during Application_Start or Application_End will throw exception.
                     * This behavior is intentional from microsoft
                     * See https://stackoverflow.com/questions/2518057/request-is-not-available-in-this-context/23908099#comment2514887_2518066
                     */
                    Logger.Warn("HttpContext.Request access when it is not suppose to", ex);
                    return CancellationToken.None;
                }
            }
        }
        public HttpContextCancellationTokenProvider(
            IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
            : base(cancellationTokenOverrideScopeProvider)
        {
        }
    }
}
