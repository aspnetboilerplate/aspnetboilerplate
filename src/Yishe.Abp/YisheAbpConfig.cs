using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yishe.Abp
{
   
    public class YisheAbpConfig
    {
        private string redisConnectionStringKey = "Redis.ServerConnectionString";

        public string RedisConnectionStringKey
        {
            get { return redisConnectionStringKey; }
            set { redisConnectionStringKey = value; }
        }


    }

    public static class YisheAbpConfigExtensions
    {
        public static YisheAbpConfig AbpRedisCacheModule(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration
                .GetOrCreate("YisheAbpModule",
                    () => moduleConfigurations.AbpConfiguration.IocManager.Resolve<YisheAbpConfig>()
                );
        }
    }
}
