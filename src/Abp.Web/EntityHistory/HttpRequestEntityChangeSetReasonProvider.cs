using System;
using Abp.Dependency;
using Abp.EntityHistory;
using Abp.Runtime;
using JetBrains.Annotations;
using System.Web;

namespace Abp.Web.EntityHistory
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
                    return HttpContext.Current?.Request.Url.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                    return null;
                }
            }
        }

        public HttpRequestEntityChangeSetReasonProvider(
            IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider
            ) : base(reasonOverrideScopeProvider)
        {
        }
    }
}
