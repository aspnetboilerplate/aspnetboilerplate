using Abp.Runtime;
using Castle.Core.Logging;
using System;

namespace Abp.EntityHistory
{
    public abstract class EntityChangeSetReasonProviderBase : IEntityChangeSetReasonProvider
    {
        public const string ReasonOverrideContextKey = "Abp.EntityHistory.Reason.Override";

        public abstract string Reason { get; }

        public ILogger Logger { get; set; }

        protected ReasonOverride OverridedValue => ReasonOverrideScopeProvider.GetValue(ReasonOverrideContextKey);
        protected IAmbientScopeProvider<ReasonOverride> ReasonOverrideScopeProvider { get; }

        protected EntityChangeSetReasonProviderBase(IAmbientScopeProvider<ReasonOverride> reasonOverrideScopeProvider)
        {
            ReasonOverrideScopeProvider = reasonOverrideScopeProvider;
            Logger = NullLogger.Instance;
        }

        public IDisposable Use(string reason)
        {
            return ReasonOverrideScopeProvider.BeginScope(ReasonOverrideContextKey, new ReasonOverride(reason));
        }
    }
}
