using System;
using System.Threading;
using Abp.CachedUniqueKeys;
using Abp.Modules;
using Abp.Runtime.Caching;
using Abp.Threading;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.CachedUniqueKeys
{
    public class CachedUniqueKeyPerUser_Tests : SampleAppTestBase<CachedUniqueKeyPerUserTestModule>
    {
        public const string MyTestCacheName = "MyTestCacheName";
        public const string MyTestCacheName2 = "MyTestCacheName2";
        private User user1;
        private User user2;
        private ICachedUniqueKeyPerUser _cachedUniqueKeyPerUser;
        private ICacheManager _cacheManager;

        public CachedUniqueKeyPerUser_Tests()
        {
            user1 = CreateUser("User1");
            user2 = CreateUser("User2");

            AbpSession.UserId = user1.Id;
            AbpSession.TenantId = user1.TenantId;

            _cachedUniqueKeyPerUser = Resolve<ICachedUniqueKeyPerUser>();
            _cacheManager = Resolve<ICacheManager>();
        }

        [Fact]
        public void Should_Get_Same_Key_Until_Cache_Expire()
        {
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey2.ShouldBe(cachedKey);

            Thread.Sleep(TimeSpan.FromSeconds(6));

            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey3.ShouldNotBe(cachedKey);
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey4.ShouldBe(cachedKey3);
        }

        [Fact]
        public void Should_Get_New_Key_If_Cache_Cleared()
        {
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey2.ShouldBe(cachedKey);

            _cacheManager.GetCache(MyTestCacheName).Clear();

            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey3.ShouldNotBe(cachedKey);
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey4.ShouldBe(cachedKey3);
        }

        [Fact]
        public void Should_Get_Different_Keys_For_Different_Users()
        {
            AbpSession.UserId = user1.Id;
            AbpSession.UserId = user1.Id;
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            
            AbpSession.UserId = user2.Id;
            AbpSession.UserId = user2.Id;
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey2.ShouldNotBe(cachedKey);
            
            AbpSession.UserId = user1.Id;
            AbpSession.UserId = user1.Id;
            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey3.ShouldBe(cachedKey);
            
            AbpSession.UserId = user2.Id;
            AbpSession.UserId = user2.Id;
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey4.ShouldBe(cachedKey2);
        }
        
        [Fact]
        public void Should_Get_Different_Keys_For_Different_CacheNames()
        {
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName2);
            cachedKey2.ShouldNotBe(cachedKey);
            
            AbpSession.UserId = user2.Id;
            AbpSession.UserId = user2.Id;
            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName2);
            cachedKey3.ShouldNotBe(cachedKey4);
        }

        [Fact]
        public void Should_Remove_Key_And_Give_New_Key()
        {
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey2.ShouldBe(cachedKey);

            _cachedUniqueKeyPerUser.RemoveKey(MyTestCacheName);

            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey3.ShouldNotBe(cachedKey);
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey4.ShouldBe(cachedKey3);
        }

        [Fact]
        public void Should_Clear_Cache_And_Give_New_Key()
        {
            var cachedKey = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            var cachedKey2 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey2.ShouldBe(cachedKey);

            _cachedUniqueKeyPerUser.ClearCache(MyTestCacheName);

            var cachedKey3 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey3.ShouldNotBe(cachedKey);
            var cachedKey4 = _cachedUniqueKeyPerUser.GetKey(MyTestCacheName);
            cachedKey4.ShouldBe(cachedKey3);
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class CachedUniqueKeyPerUserTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CachedUniqueKeyPerUserTestModule).Assembly);

            Configuration.Caching.Configure(CachedUniqueKeyPerUser_Tests.MyTestCacheName, cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromSeconds(5); });
        }
    }
}
