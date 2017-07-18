using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Runtime.Caching.Redis
{
    public static class AbpRedisCacheOptionsExtensions
    {
        public static void UseProtoBuf(this AbpRedisCacheOptions options)
        {
            options.AbpStartupConfiguration
                .ReplaceService<IRedisCacheSerializer, ProtoBufRedisCacheSerializer>(DependencyLifeStyle.Transient);
        }
    }
}
