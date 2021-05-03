### Cached Unique Key Per User

Cached Unique Key Per User is an implementation that you can create a unique key per user. It can be used in cases where you need a temporary unique key for the user. It creates a unique key for a given user for a given key name, and stores it until the cache expires. You can use it when you need a temporary unique key for the user.

Keys created with `ICachedUniqueKeyPerUser` can be expired at any time, or automatically expired at the end of the cache time. 

Example Usages:

* When a user starts a transaction, you can create a unique key for this transaction and keep transaction continuity with this transaction unique key in subsequent requests. 

* You can add a unique key to your big data responses from the server with this unique key. Then whenever your client needs that big data, it may just request the server to check whether there is any change in this data with the unique key. Then you can expire this key when the data changes to force the user to request the data again.

We currently use `ICachedUniqueKeyPerUser` to cache the `GetScripts` results which we create client-side js on the server depending on certain situations of the user. When there is a change in the user's state that affects `GetScripts` result, we expire this key to force the client to send `GetScripts` request to the server again.

It uses [Caching](/Pages/Documents/Caching) system to store keys.

#### ICachedUniqueKeyPerUser

​	The main interface for cached unique key per user. We can inject it and use it to manage keys. Example:

```html
@using Abp.CachedUniqueKeys
@inject ICachedUniqueKeyPerUser CachedUniqueKeyPerUser

<script src="~/AbpScripts/GetScripts?v=@(await CachedUniqueKeyPerUser.GetKeyAsync("GetScriptsResponsePerUserCache"))" type="text/javascript"></script>
```

```csharp
public class AccountController: AbpController
{
	ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;
	public AccountController(ICachedUniqueKeyPerUser cachedUniqueKeyPerUser)
	{
		_cachedUniqueKeyPerUser = cachedUniqueKeyPerUser;
	}
	
	[UnitOfWork]
	public virtual async Task<ActionResult> ImpersonateSignIn(string tokenId)
	{
		await _cachedUniqueKeyPerUser.RemoveKey("GetScriptsResponsePerUserCache");
		//will force current user's client to request AbpScripts/GetScripts again. Other user's client caches will not be effected		
	}
}
```

* `await CachedUniqueKeyPerUser.GetKeyAsync("GetScriptsResponsePerUserCache")`: It requests a new unique key for current user with given cache name. (Cache name is `GetScriptsResponsePerUserCache`). If there is a unique key stored/non-expired, `ICachedUniqueKeyPerUser` returns it.  Otherwise it creates new unique key and returns it.
* `await _cachedUniqueKeyPerUser.RemoveKey("GetScriptsResponsePerUserCache");` removes existing key for current user.

Since it uses caching system you can use all caching configurations. Example:

```csharp
Configuration.Caching.UseRedis();//You can store unique keys anywhere you want
//...
Configuration.Caching.Configure("GetScriptsResponsePerUserCache", cache =>
{
    cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30);
});
```



***ICachedUniqueKeyPerUser.cs***

``````csharp
public interface ICachedUniqueKeyPerUser
{
	Task<string> GetKeyAsync(string cacheName);//returns key for current user
	Task<string> GetKeyAsync(string cacheName, UserIdentifier user);//returns key for given user
	Task<string> GetKeyAsync(string cacheName, int? tenantId, long? userId);//returns key for given user
	
	Task RemoveKeyAsync(string cacheName);//removes current user's key if exists
	Task RemoveKeyAsync(string cacheName, UserIdentifier user);//removes given user's key if exists
	Task RemoveKeyAsync(string cacheName, int? tenantId, long? userId);//removes given user's key if exists

	string GetKey(string cacheName);//returns key for current user
	string GetKey(string cacheName, UserIdentifier user);//returns key for given user
	string GetKey(string cacheName, int? tenantId, long? userId);//returns key for given user
	
	void RemoveKey(string cacheName);//removes current user's key if exists
	void RemoveKey(string cacheName, UserIdentifier user);//removes given user's key if exists
	void RemoveKey(string cacheName, int? tenantId, long? userId);//removes given user's key if exists

	void ClearCache(string cacheName);//removes all user's keys with clearing cache
	Task ClearCacheAsync(string cacheName);	//removes all user's keys with clearing cache
}
``````



_____



#### IGetScriptsResponsePerUser

​	ASP.NET Boilerplate provides an implementation to cache `GetScripts` response. (It requires `Abp.AspNetCore.Mvc.Caching` nuget package).

*Startup.cs*

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
{
	//...
	app.UseGetScriptsResponsePerUserCache();
```

*_Layout.cshtml*

````html
@using Abp.AspNetCore.Mvc.Caching

@inject ICachedUniqueKeyPerUser CachedUniqueKeyPerUser
@inject IGetScriptsResponsePerUserConfiguration GetScriptsResponsePerUserConfiguration

@if (GetScriptsResponsePerUserConfiguration.IsEnabled)
{
    <script src="@(ApplicationPath)AbpScripts/GetScripts?v=@(await CachedUniqueKeyPerUser.GetKeyAsync(GetScriptsResponsePerUserCache.CacheName))" type="text/javascript"></script>
}
else
{
    <script src="@(ApplicationPath)AbpScripts/GetScripts?v=@(AppTimes.StartupTime.Ticks)" type="text/javascript"></script>
}
````

*[YOURAPPNAME]WebMvcModule.cs*

```csharp
public override void PreInitialize()
{
	//...
	Configuration.Get<IGetScriptsResponsePerUserConfiguration>().IsEnabled = true;
	Configuration.Get<IGetScriptsResponsePerUserConfiguration>().MaxAge = TimeSpan.FromMinutes(30);//after that client will request again evenif uniquekey not expired
}
```

It will automatically expire `GetScript` cache when it is necessary. You can also expire it manually. Example:

```csharp
public virtual async Task<ActionResult> ImpersonateSignIn()
{
	await ClearGetScriptsResponsePerUserCache();
	//...
}

private async Task ClearGetScriptsResponsePerUserCache()
{
	if (!_getScriptsResponsePerUserConfiguration.IsEnabled)
	{
		return;
	}

	await _cachedUniqueKeyPerUser.RemoveKeyAsync(GetScriptsResponsePerUserCache.CacheName);
}
```

