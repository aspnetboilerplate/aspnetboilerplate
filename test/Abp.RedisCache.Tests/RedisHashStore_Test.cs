using Abp.Runtime.Caching.Redis;
using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.RedisCache.Tests
{
    public class RedisHashStore_Test : AbpIntegratedTestBase<AbpRedisCacheTestModule>
    {
        public class MyTestClass
        {
            public string MyProp { get; set; }
        }

        private IAbpRedisHashStore<string, MyTestClass> _abpRedisHashStore;
        public RedisHashStore_Test()
        {
            var hashStoreProvider = LocalIocManager.Resolve<IAbpRedisHashStoreProvider>();
            _abpRedisHashStore = hashStoreProvider.GetAbpRedisHashStore<string, MyTestClass>("MyStoreName");
        }
        [Fact]
        public void Should_Add_Store()
        {
            _abpRedisHashStore.Clear();

            string itemKey = "myKey";

            _abpRedisHashStore.Add(itemKey, new MyTestClass() { MyProp = "Test" });

            _abpRedisHashStore.ContainsKey(itemKey);

            _abpRedisHashStore.TryGetValue(itemKey, out MyTestClass insertedValue).ShouldBeTrue();

            insertedValue.ShouldNotBeNull();

            insertedValue.MyProp.ShouldBe("Test");
        }
        [Fact]
        public void Should_Update_Store()
        {
            _abpRedisHashStore.Clear();

            string itemKey = "myKey";

            _abpRedisHashStore.Add(itemKey, new MyTestClass() { MyProp = "Test" });

            _abpRedisHashStore.ContainsKey(itemKey);

            _abpRedisHashStore.TryGetValue(itemKey, out MyTestClass insertedValue).ShouldBeTrue();

            insertedValue.ShouldNotBeNull();

            insertedValue.MyProp.ShouldBe("Test");

            insertedValue.MyProp = "Test2";

            _abpRedisHashStore.Update(itemKey, insertedValue);

            _abpRedisHashStore.TryGetValue(itemKey, out MyTestClass updatedValue).ShouldBeTrue();

            updatedValue.ShouldNotBeNull();

            updatedValue.MyProp.ShouldBe("Test2");
        }
        [Fact]
        public void Should_Clear_Store()
        {
            _abpRedisHashStore.Clear();

            string itemKey = "myKey";

            _abpRedisHashStore.Add(itemKey, new MyTestClass() { MyProp = "Test" });

            _abpRedisHashStore.Count.ShouldBe(1);

            _abpRedisHashStore.Clear();

            _abpRedisHashStore.Count.ShouldBe(0);
        }

        [Fact]
        public void Should_Remove_Store()
        {
            _abpRedisHashStore.Clear();

            string itemKey = "myKey";

            _abpRedisHashStore.Add(itemKey, new MyTestClass() { MyProp = "Test" });

            _abpRedisHashStore.ContainsKey(itemKey).ShouldBeTrue();

            _abpRedisHashStore.Remove(itemKey);
            _abpRedisHashStore.ContainsKey(itemKey).ShouldBeFalse();

            _abpRedisHashStore.Count.ShouldBe(0);
        }

        [Fact]
        public void Test_All()
        {
            _abpRedisHashStore.Clear();

            string itemKey = "myKey";
            string itemKey2 = "myKey2";

            var item1 = new MyTestClass() { MyProp = "Test" };
            var item2 = new MyTestClass() { MyProp = "Test2" };

            _abpRedisHashStore.Add(itemKey, item1);
            _abpRedisHashStore.TryAdd(itemKey2, item2).ShouldBeTrue();

            _abpRedisHashStore.ContainsKey(itemKey).ShouldBeTrue();
            _abpRedisHashStore.Count.ShouldBe(2);

            _abpRedisHashStore.TryGetValue(itemKey, out MyTestClass inserted);

            inserted.MyProp.ShouldBe(item1.MyProp);

            _abpRedisHashStore.TryGetValue(itemKey2, out MyTestClass inserted2);

            inserted2.MyProp.ShouldBe(item2.MyProp);

            var _allKeys = _abpRedisHashStore.GetAllKeys();
            _allKeys.Count.ShouldBe(2);
            _allKeys.ShouldContain(itemKey);
            _allKeys.ShouldContain(itemKey2);

            _abpRedisHashStore.Remove(itemKey);
        }
    }
}
