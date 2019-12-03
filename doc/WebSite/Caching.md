### Introduction

ASP.NET Boilerplate provides an abstraction for caching. It internally
uses this cache abstraction. While the default implementation uses
[MemoryCache](https://msdn.microsoft.com/en-us/library/system.runtime.caching.memorycache(v=vs.110).aspx?f=255&MSPPError=-2147217396),
it can be implemented and swapped out with any other caching provider.
The [Abp.RedisCache](https://www.nuget.org/packages/Abp.RedisCache) package
implements cache using Redis, for instance (see the "Redis Cache Integration"
section below).

### ICacheManager

The main interface for caching is **ICacheManager**. We can
[inject](/Pages/Documents/Dependency-Injection) it and use it to get a
cache. Example:

```csharp
public class TestAppService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public TestAppService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public Item GetItem(int id)
    {
        //Try to get from cache
        return _cacheManager
                .GetCache("MyCache")
                .Get(id.ToString(), () => GetFromDatabase(id)) as Item;
    }

    public Item GetFromDatabase(int id)
    {
        //... retrieve item from database
    }
}
```

In this example, we're injecting **ICacheManager** and getting a cache
named **MyCache**. Cache names are case sensitive, that means "MyCache"
and "MYCACHE" are two different caches.

### ICache

The ICacheManager.**GetCache** method returns an **ICache**. A cache is a
singleton (per cache name). It is created the first time it's requested,
and then the same cache object is always returned. This way we can share the same cache
with the same name in different classes (clients).

In the sample code, we see a simple usage of the ICache.**Get** method. It has
two arguments:

-   **key**: A unique key (string) of an item in the cache.
-   **factory**: An action which is called if there is no item with the
    given key. The Factory method should create and return the actual item.
    This is not called if the given key is present in the cache.

The ICache interface also has methods like **GetOrDefault**, **Set**,
**Remove** and **Clear**. There are also **async** versions for all
methods.

#### ITypedCache

The **ICache** interface uses a **string** as the key and an **object** as the value.
**ITypedCache** is a wrapper to ICache to provide a **type safe**, generic
cache. We can use the generic GetCache extension method to get an
ITypedCache:

```csharp
ITypedCache<int, Item> myCache = _cacheManager.GetCache<int, Item>("MyCache");
```

We can also use the **AsTyped** extension method to convert an existing
ICache instance to ITypedCache.

### Configuration

The default cache expiration time is 60 minutes. It's sliding, so if you don't
use an item in the cache for 60 minutes, it's automatically removed from
the cache. You can configure it for all caches or for a specific cache.

```csharp
//Configuration for all caches
Configuration.Caching.ConfigureAll(cache =>
{
    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(2);
});

//Configuration for a specific cache
Configuration.Caching.Configure("MyCache", cache =>
{
    cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
});
```

This code should be placed in the
[**PreInitialize**](/Pages/Documents/Module-System#preinitialize)
method of your module. With this code, "MyCache" will expire in 8 hours
while all other cache items will expire in 2 hours.

Your configuration action is called once the cache is first created (on
first request). Configuration is not restricted to
DefaultSlidingExpireTime only, since the cache object is an **ICacheOptions**, you can
use it's properties to freely configure and initialize it.

### Entity Caching

While ASP.NET Boilerplate's cache system is for general purposes, there is an
**EntityCache** base class that can help you if you want to cache
entities. We can use this base class if we get entities by their Ids and
we want to **cache them by Id**, so as to not query from the database repeatedly.
Assume that we have a Person entity like that:

```csharp
public class Person : Entity
{
    public string Name { get; set; }

    public int Age { get; set; }
}
```

Assume that we frequently want to get the **Name** of people while we
know their **Id**. First, we create a class to store **cache
items**:

```csharp
[AutoMapFrom(typeof(Person))]
public class PersonCacheItem
{
    public string Name { get; set; }
}
```

**Do not directly store entities in the cache**, since caching may
need to **serialize** cached objects. Entities may not be serialized,
especially if they have navigation properties. That's why we defined a
simple ([DTO](Data-Transfer-Objects.md)) class to store data in
the cache. We added the **AutoMapFrom** attribute since we want to use
AutoMapper to automatically convert the Person entities to the PersonCacheItem objects.
If we don't use AutoMapper, we should **override the
MapToCacheItem** method of the EntityCache class to manually convert/map it.

While it's **not required**, we may want to define an interface for our
cache class:

```csharp
public interface IPersonCache : IEntityCache<PersonCacheItem>
{

}
```

Finally, we can create the cache class to cache Person entities:

```csharp
public class PersonCache : EntityCache<Person, PersonCacheItem>, IPersonCache, ITransientDependency
{
    public PersonCache(ICacheManager cacheManager, IRepository<Person> repository)
        : base(cacheManager, repository)
    {

    }
}
```

That's it. Our person cache is ready to use! Cache class can be
transient (as in this example) or a singleton. This does not mean the
cached data is transient. It's always cached globally and accessed in a
thread-safe manner in your application.

Whenever we need the **Name** of a person, we can get it from the cache by using
the person's **Id**. Here's an example class that uses the Person cache:

```csharp
public class MyPersonService : ITransientDependency
{
    private readonly IPersonCache _personCache;

    public MyPersonService(IPersonCache personCache)
    {
        _personCache = personCache;
    }

    public string GetPersonNameById(int id)
    {
        return _personCache[id].Name; //alternative: _personCache.Get(id).Name;
    }
}
```

We simply [injected](Dependency-Injection.md) IPersonCache, got the
cache item and then got the Name property.

#### How EntityCache Works

-   It gets the entity from the repository (the database) in it's first call. It then
    gets from the cache in subsequent calls.
-   It automatically invalidates a cached entity if this entity is updated
    or deleted. Thus, it will be retrieved from the database in the next
    call.
-   It uses IObjectMapper to map an entity to a cache item. IObjectMapper is
    implemented by the AutoMapper module. You need the [AutoMapper
    module](/Pages/Documents/Data-Transfer-Objects)
    if you are using it. You can override the MapToCacheItem method to
    manually map an entity to a cache item.
-   It uses the cache class's FullName as a cache name. You can change it by
    passing a cache name to the base constructor.
-   It's thread-safe.

If you need more complex caching requirements, you can extend
EntityCache or create your own solution.

### Multi-Tenancy Entity Caching

While **EntityCache** can help you cache entities, it is not multi-tenancy safe.
For example, an entity that is retrieved and cached by tenant A should not be cached for tenant B.
To cache multi-tenancy entity correctly, we introduce **MustHaveTenantEntityCache** and **MayHaveTenantEntityCache**
which accept an entity class that implements **IMustHaveTenant** or **IMayHaveTenant** interface.

Similar to entity caching, we can have **IMayHaveTenant** entity and cache item like this:

```csharp
public class Phone : Entity, IMayHaveTenant
{
    public int? TenantId { get; set; }

    public string Number { get; set; }
}
```


```csharp
[AutoMapFrom(typeof(Phone))]
public class PhoneCacheItem
{
    public string Number { get; set; }
}
```

Similar to entity caching, it is optional to define an interface for the
cache class:

```csharp
public interface IPhoneCache : IMultiTenancyEntityCache<PhoneCacheItem>
{
}
```

Then, create cache class to cache Phone entities:

```csharp
public class PhoneCache : MayHaveTenantEntityCache<Phone, PhoneCacheItem>, IPhoneCache, ITransientDependency
{
    public PhoneCache(ICacheManager cacheManager, IUnitOfWorkManager unitOfWorkManager, IRepository<Phone> repository)
        : base(cacheManager, unitOfWorkManager, repository)
    {
    }
}
```

Now we can access Phone entity cache in a multi-tenancy safe manner in your application.
It also has all the benefits of **EntityCache**, e.g. cache globally, cache class can be transient/singleton.


#### How MustHaveTenantEntityCache/MayHaveTenantEntityCache Works

It works similar to [How EntityCache Works](#how-entitycache-works), with some differences.
-   It uses **TenantId** when constructing the cache key, e.g. "{EntityId}@{TenantId}".
-   It's multi-tenancy safe.

If you need more complex multi-tenancy caching requirements, you can extend
**MultiTenancyEntityCache** and add your own solution.

### Redis Cache Integration

The default cache manager uses **in-memory** caches. It can turn in to a problem
if you have more than one concurrent web server running the same
application. In that case, you may want a **distributed/central
cache** server. You can easily use Redis as your cache server.

First, you need to install the
[**Abp.RedisCache**](https://www.nuget.org/packages/Abp.RedisCache)
NuGet package to your application (you can install it to your Web
project, for example). Then you need to add a **DependsOn** attribute
for the **AbpRedisCacheModule** and call the **UseRedis** extension method in the
**PreInitialize** method of your [module](Module-System.md), as shown
below:

```csharp
//...other namespaces
using Abp.Runtime.Caching.Redis;

namespace MyProject.AbpZeroTemplate.Web
{
    [DependsOn(
        //...other module dependencies
        typeof(AbpRedisCacheModule))]
    public class MyProjectWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            //...other configurations

            Configuration.Caching.UseRedis();
        }

        //...other code
    }
}
```

The Abp.RedisCache package uses "**localhost**" as the **connection string** by
default. You can add a connection string to your config file to override
it. Example:

```xml
<add name="Abp.Redis.Cache" connectionString="localhost"/>
```

Also, you can add a setting to appSettings to set the database id of Redis.
Example:

```xml
<add key="Abp.Redis.Cache.DatabaseId" value="2"/>
```

For ASP.NET Core you can override it with the delegate parameter of UseRedis. Example:

```csharp
Configuration.Caching.UseRedis(options =>
{
    options.ConnectionString = _appConfiguration["RedisCache:ConnectionString"];
    options.DatabaseId = _appConfiguration.GetValue<int>("RedisCache:DatabaseId");
});
```

Different database ids are useful to create different key spaces
(isolated caches) in same server.

The **UseRedis** method also has an overload that takes an action to
directly set option values (this overrides values in the config file).

See the [Redis documentation](http://redis.io/documentation) for more
information on Redis and it's configuration.

**Note**: The Redis server should be installed and running to use the Redis
cache in ABP.
