using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Modules;
using Abp.Runtime.Caching.Redis;

namespace Abp.RedisCache.Tests
{
    [DependsOn(typeof(AbpRedisCacheModule))]
    public class AbpRedisCacheTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            //enable redis cache and OnlineClientStore with redis
            Configuration.Caching.UseRedis(options => { options.UseOnlineClientStoreWithRedisCache = true; });
        }
    }
}
