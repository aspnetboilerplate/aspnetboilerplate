using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public class NullFeatureChecker : IFeatureChecker
    {
        public static NullFeatureChecker Instance { get { return SingletonInstance; } }
        private static readonly NullFeatureChecker SingletonInstance = new NullFeatureChecker();

        public Task<bool> IsEnabledAsync(string name)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsEnabledAsync(int tenantId, string name)
        {
            return Task.FromResult(true);
        }

        public Task<string> GetValueAsync(string name)
        {
            return Task.FromResult(string.Empty);
        }

        public Task<string> GetValueAsync(int tenantId, string name)
        {
            return Task.FromResult(string.Empty);
        }
    }
}