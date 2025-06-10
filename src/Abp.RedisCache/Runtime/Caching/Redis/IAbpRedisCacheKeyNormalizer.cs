using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Runtime.Caching.Redis
{
    public interface IAbpRedisCacheKeyNormalizer
    {
        string NormalizeKey(AbpRedisCacheKeyNormalizeArgs args);
    }
}
