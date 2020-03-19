using System;
using System.Threading;
using Abp.Runtime;
using Castle.Core.Logging;

namespace Abp.Threading
{
    public abstract class CancellationTokenProviderBase : ICancellationTokenProvider
    {
        public const string CancellationTokenOverrideContextKey = "Abp.Threading.CancellationToken.Override";

        public abstract CancellationToken Token { get; }

        public ILogger Logger { get; set; }

        protected IAmbientScopeProvider<CancellationTokenOverride> CancellationTokenOverrideScopeProvider { get; }

        protected CancellationTokenOverride OverridedValue => CancellationTokenOverrideScopeProvider.GetValue(CancellationTokenOverrideContextKey);

        protected CancellationTokenProviderBase(IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
        {
            CancellationTokenOverrideScopeProvider = cancellationTokenOverrideScopeProvider;
            Logger = NullLogger.Instance;
        }

        public IDisposable Use(CancellationToken cancellationToken)
        {
            return CancellationTokenOverrideScopeProvider.BeginScope(CancellationTokenOverrideContextKey, new CancellationTokenOverride(cancellationToken));
        }
    }
}
