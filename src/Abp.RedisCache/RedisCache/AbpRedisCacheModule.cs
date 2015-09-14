using Abp.Modules;
using Abp.RedisCache.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RedisCache
{
    public class AbpRedisCacheModule : AbpModule
    {

      

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
