### Per Request Redis Cache

When you use a cache object derived from the default Redis cache implementation (`ICacheManager` with Redis), every action you make is executed on Redis. Although Redis works much faster than many operations such as reading from the database, making an API query, Redis queries have a performance cost (this cost may vary depending on the location of your Redis server to your server). If your project uses too many Redis queries, this can sometimes cause performance issues and also cause a bottleneck. One method that can solve this is to cache these Redis queries locally for some non-critical data you query from Redis many times in your project.

For example, let's say you are caching about localization, and you keep the dictionary of these localization keys in Redis. You may need to go to Redis dozens of times and read the value of the relevant localization key from Redis for the localization process on each page load. Instead, you can pull that data from the Redis once and make queries on it. You can use `IAbpPerRequestRedisCacheManager` implementation instead of `ICacheManager` (with Redis) for data whose instant change is not critical.

Using `IAbpPerRequestRedisCacheManager` is the same as using `ICacheManager` and returns objects of the same data type. The only difference is that when a cache object is created using `IAbpPerRequestRedisCacheManager`, Redis queries are cached locally on the `HttpContext`. Thus, a query is sent to Redis only once within a single HTTP request. Repeated queries that follow do not go to Redis, and data from previous Redis response is used. In this way, you can improve performance.

### Usage of IAbpPerRequestRedisCacheManager

* Import `Abp.AspNetCore.PerRequestRedisCache` NuGet package. 

* Add depends on `AbpAspNetCorePerRequestRedisCacheModule` 

  ```csharp
  [DependsOn(typeof(AbpAspNetCorePerRequestRedisCacheModule))]
  public class MyModule : AbpModule
  {
  //...
  ```

* Then get cache using `IAbpPerRequestRedisCacheManager`

  ```csharp
  public class TestAppService : ApplicationService
  {
      private readonly IAbpPerRequestRedisCacheManager _cacheManager;
  
      public TestAppService(IAbpPerRequestRedisCacheManager cacheManager)
      {
          _cacheManager = cacheManager;
      }
      
      private ICache GetMyCache() => _cacheManager.GetCache("MyCache"); // get cache using `IAbpPerRequestRedisCacheManager`
  ```

  It is all you have to do in order to use per request Redis cache implementation. Now you can use all caching features. See caching [documentation](Caching.md) for more information.

Note: You must enable Redis to use `IAbpPerRequestRedisCacheManager`. ([Caching documentation](Caching#redis-cache-integration))

### Replace ICacheManager with IAbpPerRequestRedisCacheManager

In order to replace current implementation of the cache with `PerRequestRedisCache`, after you import NuGet package and add depends on, enable Redis with `usePerRequestRedisCache: true` parameter

```csharp
Configuration.Caching.UseRedis(usePerRequestRedisCache: true);
```

This will replace `ICacheManager` with `IAbpPerRequestRedisCacheManager`. Your entire project will start working with `IAbpPerRequestRedisCacheManager`.

[GitHub link](https://github.com/aspnetboilerplate/aspnetboilerplate/tree/dev/src/Abp.AspNetCore.PerRequestRedisCache)
