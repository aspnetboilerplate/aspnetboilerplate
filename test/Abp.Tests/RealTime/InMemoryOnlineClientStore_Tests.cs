using System;
using Abp.RealTime;
using Shouldly;
using Xunit;

namespace Abp.Tests.RealTime
{
    public class InMemoryOnlineClientStore_Tests
    {
        private readonly InMemoryOnlineClientStore _store;

        public InMemoryOnlineClientStore_Tests()
        {
            _store = new InMemoryOnlineClientStore();
        }

        [Fact]
        public void Test_All()
        {
            var connectionId = Guid.NewGuid().ToString("N");

            _store.Add(new OnlineClient(connectionId, "127.0.0.1", 1, 2));
            _store.TryGet(connectionId, out IOnlineClient client).ShouldBeTrue();

            _store.Contains(connectionId).ShouldBeTrue();
            _store.GetAll().Count.ShouldBe(1);
            _store.Remove(connectionId).ShouldBeTrue();
            _store.GetAll().Count.ShouldBe(0);
        }
    }
}
