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
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                string localizedKey = GetPerRequestRedisCacheKey(key);

                if (httpContext.Items.ContainsKey(localizedKey))
                {
                    var conditionalValue = (ConditionalValue<object>) httpContext.Items[localizedKey];
                    value = conditionalValue.HasValue ? conditionalValue.Value : null;

                    return conditionalValue.HasValue;
                }
                else
                {
                    bool hasValue = base.TryGetValue(key, out value);
                    httpContext.Items[localizedKey] = new ConditionalValue<object>(hasValue, hasValue ? value : null);

                    return hasValue;
                }
            }

            return base.TryGetValue(key, out value);
        }

        public override ConditionalValue<object>[] TryGetValues(string[] keys)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                Dictionary<string, string> localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

                string[] missingKeys = localizedKeys
                    .Where(kv => !httpContext.Items.ContainsKey(kv.Key))
                    .Select(kv => kv.Value)
                    .ToArray();

                var missingValues = base.TryGetValues(missingKeys);

                for (int i = 0; i < missingKeys.Length; i++)
                {
                    httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
                }

                List<ConditionalValue<object>> result = new List<ConditionalValue<object>>();

                foreach (string localizedKey in localizedKeys.Keys)
                {
                    result.Add((ConditionalValue<object>) httpContext.Items[localizedKey]);
                }

                return result.ToArray();
            }

            return base.TryGetValues(keys);
        }

        public override async Task<ConditionalValue<object>> TryGetValueAsync(string key)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                string localizedKey = GetPerRequestRedisCacheKey(key);

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

            return await base.TryGetValueAsync(key);
        }

        public override async Task<ConditionalValue<object>[]> TryGetValuesAsync(string[] keys)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                Dictionary<string, string> localizedKeys = keys.ToDictionary(GetPerRequestRedisCacheKey);

                string[] missingKeys = localizedKeys
                    .Where(kv => !httpContext.Items.ContainsKey(kv.Key))
                    .Select(kv => kv.Value)
                    .ToArray();

                var missingValues = await base.TryGetValuesAsync(missingKeys);

                for (int i = 0; i < missingKeys.Length; i++)
                {
                    httpContext.Items[GetPerRequestRedisCacheKey(missingKeys[i])] = missingValues[i];
                }

                List<ConditionalValue<object>> result = new List<ConditionalValue<object>>();

                foreach (string localizedKey in localizedKeys.Keys)
                {
                    result.Add((ConditionalValue<object>) httpContext.Items[localizedKey]);
                }

                return result.ToArray();
            }

            return await base.TryGetValuesAsync(keys);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            base.Set(key, value, slidingExpireTime, absoluteExpireTime);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
            }
        }

        public override async Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            await base.SetAsync(key, value, slidingExpireTime, absoluteExpireTime);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                httpContext.Items[GetPerRequestRedisCacheKey(key)] = new ConditionalValue<object>(true, value);
            }
        }

        public override void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            base.Set(pairs, slidingExpireTime, absoluteExpireTime);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                for (int i = 0; i < pairs.Length; i++)
                {
                    httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
                }
            }
        }

        public override async Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            await base.SetAsync(pairs, slidingExpireTime, absoluteExpireTime);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                for (int i = 0; i < pairs.Length; i++)
                {
                    httpContext.Items[GetPerRequestRedisCacheKey(pairs[i].Key)] = new ConditionalValue<object>(true, pairs[i].Value);
                }
            }
        }

        public override void Remove(string key)
        {
            base.Remove(key);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                string localizedKey = GetPerRequestRedisCacheKey(key);

                if (!httpContext.Items.ContainsKey(localizedKey))
                {
                    httpContext.Items.Remove(localizedKey);
                }
            }
        }

        public override async Task RemoveAsync(string key)
        {
            await base.RemoveAsync(key);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                string localizedKey = GetPerRequestRedisCacheKey(key);

                if (!httpContext.Items.ContainsKey(localizedKey))
                {
                    httpContext.Items.Remove(localizedKey);
                }
            }
        }

        public override void Remove(string[] keys)
        {
            base.Remove(keys);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                foreach (string key in keys)
                {
                    string localizedKey = GetPerRequestRedisCacheKey(key);

                    if (!httpContext.Items.ContainsKey(localizedKey))
                    {
                        httpContext.Items.Remove(localizedKey);
                    }
                }
            }
        }

        public override async Task RemoveAsync(string[] keys)
        {
            await base.RemoveAsync(keys);

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                foreach (string key in keys)
                {
                    string localizedKey = GetPerRequestRedisCacheKey(key);

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

            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                string localizedKeyPrefix = GetPerRequestRedisCacheKey("");

                foreach (string key in httpContext.Items.Keys.ToList())
                {
                    if (key.StartsWith(localizedKeyPrefix))
                    {
                        httpContext.Items.Remove(key);
                    }
                }
            }
        }

        protected virtual string GetPerRequestRedisCacheKey(string key)
        {
            return AbpPerRequestRedisCachePrefix + GetLocalizedRedisKey(key).ToString();
        }
    }
}