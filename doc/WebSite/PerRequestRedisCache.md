### Per Request Redis Cache

When you use a cache object derived from the default redis cache implementation (`ICacheManager` with redis), every action you make is executed on redis. Although redis works much faster than many operations such as reading from the database, making an api query, redis queries have a performance cost (this cost may vary depending on the location of your redis server to your server). If your project use too many redis queries, this can sometimes cause performance issues and also cause a bottleneck. One method that can solve this is to cache these redis queries locally for some non-critical data that you query from redis many times in your project.

For example, let's say you are caching about localization and you keep the dictionary of these localization keys in redis.You may need to go to redis dozens of times and read the value of the relevant localization key from redis for the localization process on each page load. Instead, that data can be pulled from the redis once and queries can be made on it. You can use `IPerRequestRedisCacheManager` implementation instead of `ICacheManager`(with redis) for data whose instant change is not critical.

Using `IPerRequestRedisCacheManager` is exactly the same as using `ICacheManager` and returns objects of the same data type. The only difference is that when a cache object created using `IPerRequestRedisCacheManager`, redis queries are cached locally on the httpcontext. Thus, a query is sent to redis only once within a single http request. Repeated queries that follow do not go to redis, and data from previous redis response is used. In this way, performance can be improved.

### Usage

* Import `Abp.AspNetCore.PerRequestRedisCache` nuget package. 

* Then get cache using `IPerRequestRedisCacheManager`

  ```csharp
  public class TestAppService : ApplicationService
  {
      private readonly IPerRequestRedisCacheManager _cacheManager;
  
      public TestAppService(IPerRequestRedisCacheManager cacheManager)
      {
          _cacheManager = cacheManager;
      }
      
      private ICache GetMyCache() => _cacheManager.GetCache("MyCache"); // get cache using `IPerRequestRedisCacheManager`
  ```

  It is all you have to do in order to use per request redis cache implementation. Now you can use all caching features. See caching [documentation](Caching.md) for more information.

Not: You must enable redis to use `IPerRequestRedisCacheManager`. See caching [documentation](Caching.md#redis-cache-integration)