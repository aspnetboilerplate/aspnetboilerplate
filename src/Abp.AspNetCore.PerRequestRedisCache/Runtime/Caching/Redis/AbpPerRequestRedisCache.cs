using Abp.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching.Redis
{
    public class AbpPerRequestRedisCache : AbpRedisCache, IAbpPerRequestRedisCache
    {
        private const string AbpPerRequestRedisCachePrefix = "AbpPerRequestRedisCache:";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public AbpPerRequestRedisCache(
            string name,
            IAbpRedisCacheDatabaseProvider redisCacheDatabaseProvider,
            IRedisCacheSerializer redisCacheSerializer,
            IHttpContextAccessor httpContextAccessor)
            : base(name, redisCacheDatabaseProvider, redisCacheSerializer)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override bool TryGetValue(string key, out object value)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return base.TryGetValue(key, out value);
            }
            
            var localizedKey = GetPerRequestRedisCacheKey(key);

            if (httpContext.Items.ContainsKey(localizedKey))
            {
                var conditionalValue = (ConditionalValue<object>) httpContext.Items[localizedKey];
                value = conditionalValue.HasValue ? conditionalValue.Value : null;

                return conditionalValue.HasValue;
            }

            var hasValue = base.TryGetValue(key, out value);
            httpContext.Items[localizedKey] = new ConditionalValue<object>(hasValue, hasValue ? value : null);
            return hasValue;

        }

        public override ConditionalValue<object>[] TryGetValues(string[] keys)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return base.TryGetValues(keys);
            }
            
            var localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

            var missingKeys = localizedKeys
                .Where(kv => !httpContext.Items.ContainsKey(kv.Key))
                .Select(kv => kv.Value)
                .ToArray();

            var missingValues = base.TryGetValues(missingKeys);

            for (var i = 0; i < missingKeys.Length; i++)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
            }

            return localizedKeys.Keys.Select(localizedKey => (ConditionalValue<object>) httpContext.Items[localizedKey]).ToArray();
        }

        public override async Task<ConditionalValue<object>> TryGetValueAsync(string key)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return await base.TryGetValueAsync(key);
            }
            
            var localizedKey = GetPerRequestRedisCacheKey(key);

            if (httpContext.Items.ContainsKey(localizedKey))
            {
                var conditionalValue = (ConditionalValue<object>) httpContext.Items[localizedKey];
                return conditionalValue;
            }
            else
            {
                var conditionalValue = await base.TryGetValueAsync(key);
                httpContext.Items[localizedKey] = conditionalValue;
                return conditionalValue;
            }

        }

        public override async Task<ConditionalValue<object>[]> TryGetValuesAsync(string[] keys)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return await base.TryGetValuesAsync(keys);
            }
            
            var localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

            var missingKeys = localizedKeys
                .Where(kv => !httpContext.Items.ContainsKey(kv.Key))
                .Select(kv => kv.Value)
                .ToArray();

            var missingValues = await base.TryGetValuesAsync(missingKeys);

            for (var i = 0; i < missingKeys.Length; i++)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
            }

            return localizedKeys.Keys.Select(localizedKey => (ConditionalValue<object>) httpContext.Items[localizedKey]).ToArray();
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            base.Set(key, value, slidingExpireTime, absoluteExpireTime);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
            }
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            await base.SetAsync(key, value, slidingExpireTime, absoluteExpireTime);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
            }
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            base.Set(pairs, slidingExpireTime, absoluteExpireTime);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return;
            }
            
            for (var i = 0; i < pairs.Length; i++)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
            }
        }

        public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            await base.SetAsync(pairs, slidingExpireTime, absoluteExpireTime);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                for (var i = 0; i < pairs.Length; i++)
                {
                    httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
                }
            }
        }

        public override void Remove(string key)
        {
            base.Remove(key);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return;
            }
            
            var localizedKey = GetPerRequestRedisCacheKey(key);

            if (!httpContext.Items.ContainsKey(localizedKey))
            {
                httpContext.Items.Remove(localizedKey);
            }
        }

        public override async Task RemoveAsync(string key)
        {
            await base.RemoveAsync(key);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                var localizedKey = GetPerRequestRedisCacheKey(key);

                if (!httpContext.Items.ContainsKey(localizedKey))
                {
                    httpContext.Items.Remove(localizedKey);
                }
            }
        }

        public override void Remove(string[] keys)
        {
            base.Remove(keys);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return;
            }
            
            foreach (var key in keys)
            {
                var localizedKey = GetPerRequestRedisCacheKey(key);

                if (!httpContext.Items.ContainsKey(localizedKey))
                {
                    httpContext.Items.Remove(localizedKey);
                }
            }
        }

        public override async Task RemoveAsync(string[] keys)
        {
            await base.RemoveAsync(keys);

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                foreach (var key in keys)
                {
                    var localizedKey = GetPerRequestRedisCacheKey(key);

                    if (!httpContext.Items.ContainsKey(localizedKey))
                    {
                        httpContext.Items.Remove(localizedKey);
                    }
                }
            }
        }

        public override void Clear()
        {
            base.Clear();

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return;
            }
            
            var localizedKeyPrefix = GetPerRequestRedisCacheKey("");

            foreach (string key in httpContext.Items.Keys.ToList())
            {
                if (key.StartsWith(localizedKeyPrefix))
                {
                    httpContext.Items.Remove(key);
                }
            }
        }

        protected virtual string GetPerRequestRedisCacheKey(string key)
        {
            return AbpPerRequestRedisCachePrefix + GetLocalizedRedisKey(key).ToString();
        }
    }
}
