using System;
using System.Threading;
using Abp.Runtime;

namespace Abp.Threading
{
    public abstract class CancellationTokenProviderBase : ICancellationTokenProvider
    {
        public const string CancellationTokenOverrideContextKey = "Abp.Threading.CancellationToken.Override";
        public abstract CancellationToken Token { get; }
        protected IAmbientScopeProvider<CancellationTokenOverride> CancellationTokenOverrideScopeProvider { get; }
        protected CancellationTokenOverride OverridedValue => CancellationTokenOverrideScopeProvider.GetValue(CancellationTokenOverrideContextKey);

        protected CancellationTokenProviderBase(IAmbientScopeProvider<CancellationTokenOverride> cancellationTokenOverrideScopeProvider)
        {
            CancellationTokenOverrideScopeProvider = cancellationTokenOverrideScopeProvider;
        }

        public IDisposable Use(CancellationToken cancellationToken)
        {
            return CancellationTokenOverrideScopeProvider.BeginScope(CancellationTokenOverrideContextKey, new CancellationTokenOverride(cancellationToken));
        }
    }
}
