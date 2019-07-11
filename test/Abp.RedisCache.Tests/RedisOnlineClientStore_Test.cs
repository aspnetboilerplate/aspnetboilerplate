using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.RealTime;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Redis;
using Abp.Runtime.Caching.Redis.OnlineClientStore;
using Abp.TestBase;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.RedisCache.Tests
{
    public class RedisOnlineClientStore_Test : AbpIntegratedTestBase<AbpRedisCacheTestModule>
    {
        private IOnlineClientStore _clientStore;
        public RedisOnlineClientStore_Test()
        {
            _clientStore = LocalIocManager.Resolve<IOnlineClientStore>();
        }

        [Fact]
        public void Should_Insert_Client()
        {
            _clientStore.Clear();
            _clientStore.Add(new OnlineClient("1", "0:0:0:0", null, null));

            _clientStore.TryGet("1", out IOnlineClient onlineClient).ShouldBeTrue();

            onlineClient.ConnectionId.ShouldBe("1");
        }

        [Fact]
        public void Should_Contains_Client()
        {
            _clientStore.Clear();
            _clientStore.Add(new OnlineClient("1", "0:0:0:0", null, null));

            _clientStore.Contains("1").ShouldBeTrue();
            _clientStore.Contains("2").ShouldBeFalse();
        }
        [Fact]
        public void Should_List_All_Clients()
        {
            _clientStore.Clear();
            _clientStore.Add(new OnlineClient("1", "0:0:0:0", null, null));
            _clientStore.Add(new OnlineClient("2", "0:0:0:1", null, null));

            var allClients = _clientStore.GetAll();
            allClients.Count.ShouldBe(2);
            allClients[0].ConnectionId.ShouldBe("1");
            allClients[1].ConnectionId.ShouldBe("2");
        }

        private static readonly RNGCryptoServiceProvider _keyGenerator = new RNGCryptoServiceProvider();

        [Fact]
        public void Should_Work_With_IOnline_Client_Manager()
        {

            //Test store registration
            var store = LocalIocManager.Resolve<IOnlineClientStore>().ShouldBeOfType<AbpRedisOnlineClientStore>();

            store.Clear();

            var clientManager = LocalIocManager.Resolve<IOnlineClientManager>();

            int tenantId = 1;

            Dictionary<string, int> connections = new Dictionary<string, int>();

            for (int i = 0; i < 100; i++)
                connections.Add(MakeNewConnectionId(), i + 1);

            foreach (var pair in connections)
                clientManager.Add(new OnlineClient(pair.Key, "127.0.0.1", tenantId, pair.Value));

            var testId = connections.Keys.ToList()[5];

            clientManager.GetAllClients().Count.ShouldBe(connections.Count);
            clientManager.GetAllByUserId(new UserIdentifier(tenantId, connections[testId])).Count.ShouldBe(1);
            clientManager.GetByConnectionIdOrNull(testId).ShouldNotBeNull();
            clientManager.Remove(testId).ShouldBeTrue();
            clientManager.GetAllClients().Count.ShouldBe(connections.Count - 1);
            clientManager.GetByConnectionIdOrNull(testId).ShouldBeNull();
            clientManager.GetAllByUserId(new UserIdentifier(tenantId, connections[testId])).Count.ShouldBe(0);
        }

        private static string MakeNewConnectionId()
        {
            var buffer = new byte[16];
            _keyGenerator.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
    }
}
