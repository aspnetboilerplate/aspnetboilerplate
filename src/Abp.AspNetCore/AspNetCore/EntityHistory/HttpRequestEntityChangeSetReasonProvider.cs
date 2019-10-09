using Abp.Dependency;
using Abp.EntityHistory;
using Abp.Runtime;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace Abp.AspNetCore.EntityHistory
{
    /// <summary>
    /// Implements <see cref="IEntityChangeSetReasonProvider"/> to get reason from HTTP request.
    /// </summary>
    public class HttpRequestEntityChangeSetReasonProvider : EntityChangeSetReasonProviderBase, ISingletonDependency
    {
        [CanBeNull]
        public override string Reason
        {
            get
            {
                if (OverridedValue != null)
                {
                    return OverridedValue.Reason;
                }

                // TODO: Use back HttpContextAccessor.HttpContext?.Request.GetDisplayUrl()
                // after moved to net core 3.0
                // see https://github.com/aspnet/AspNetCore/issues/2718#issuecomment-482347489
                return GetDisplayUrl(HttpContextAccessor.HttpContext?.Request);
            }
        }

        protected IHttpContextAccessor HttpContextAccessor { get; }

        private const string SchemeDelimiter = "://";

        public HttpRequestEntityChangeSetReasonProvider(
            IHttpContextAccessor httpContextAccessor,

            IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider
            ) : base(reasonOverrideScopeProvider)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        private string GetDisplayUrl(HttpRequest request)
        {
            if (request == null)
            {
                Logger.Debug("Unable to get URL from HttpRequest, fallback to null");
                return null;
            }

            var scheme = request.Scheme ?? string.Empty;
            var host = request.Host.Value ?? string.Empty;
            var pathBase = request.PathBase.Value ?? string.Empty;
            var path = request.Path.Value ?? string.Empty;
            var queryString = request.QueryString.Value ?? string.Empty;

            // PERF: Calculate string length to allocate correct buffer size for StringBuilder.
            var length = scheme.Length + SchemeDelimiter.Length + host.Length
                + pathBase.Length + path.Length + queryString.Length;

            return new StringBuilder(length)
                .Append(scheme)
                .Append(SchemeDelimiter)
                .Append(host)
                .Append(pathBase)
                .Append(path)
                .Append(queryString)
                .ToString();
        }
    }
}
