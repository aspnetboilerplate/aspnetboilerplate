using Abp.Dependency;
using Abp.EntityHistory;
using Abp.Runtime;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System;

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

                try
                {
                    return HttpContextAccessor.HttpContext?.Request.GetDisplayUrl();
                }
                catch (NullReferenceException)
                {
                    // Workaround: https://github.com/aspnet/Home/issues/2718
                    return null;
                }
            }
        }

        protected IHttpContextAccessor HttpContextAccessor { get; }

        public HttpRequestEntityChangeSetReasonProvider(
            IHttpContextAccessor httpContextAccessor,

            IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider
            ) : base(reasonOverrideScopeProvider)
        {
            HttpContextAccessor = httpContextAccessor;
        }
    }
}
