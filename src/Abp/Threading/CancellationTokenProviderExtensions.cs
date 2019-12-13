using System.Threading;

namespace Abp.Threading
{
    public static class CancellationTokenProviderExtensions
    {
        public static CancellationToken FallbackToProvider(this ICancellationTokenProvider provider, CancellationToken preferredValue = default)
        {
            return preferredValue == default || preferredValue == CancellationToken.None
                ? provider.Token
                : preferredValue;
        }
    }
}
