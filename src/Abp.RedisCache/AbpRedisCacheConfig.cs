using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp
{
    public class AbpRedisCacheConfig
    {
        private string connectionStringKey = "Abp.Redis.Cache";

        public string ConnectionStringKey
        {
            get { return connectionStringKey; }
            set { connectionStringKey = value; }
        }


    } 

    public static class AbpRedisCacheConfigEctensions
    {
        public static AbpRedisCacheConfig AbpRedisCacheModule(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration
                .GetOrCreate("AbpRedisCacheModule",
                    () => moduleConfigurations.AbpConfiguration.IocManager.Resolve<AbpRedisCacheConfig>()
                );
        }
    }
}
