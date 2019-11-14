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

                // The code for GetDisplayUrl comes from asp net core 3.0. 
                // This method is not implemented in asp net core 2.x, so we need to implement it manually.
                // https://github.com/aspnet/AspNetCore/blob/master/src/Http/Http.Extensions/src/UriHelper.cs#L196
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
