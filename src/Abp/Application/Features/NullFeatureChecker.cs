using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public class NullFeatureValueStore : IFeatureValueStore
    {
        public static NullFeatureValueStore Instance { get { return SingletonInstance; } }
        private static readonly NullFeatureValueStore SingletonInstance = new NullFeatureValueStore();

        public Task<string> GetValueOrNullAsync(int tenantId, Feature feature)
        {
            return Task.FromResult((string) null);
        }
    }
}