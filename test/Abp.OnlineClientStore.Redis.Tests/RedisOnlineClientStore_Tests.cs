using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Abp.Configuration.Startup;
using Abp.RealTime;
using Abp.RealTime.Redis;
using Abp.TestBase;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.OnlineClientStore.Redis.Tests
{
    public class RedisOnlineClientStore_Tests : AbpIntegratedTestBase<AbpRedisOnlineClientStoreTestModule>
    {
        private static readonly RNGCryptoServiceProvider _keyGenerator = new RNGCryptoServiceProvider();
        
        [Fact]
        public void Test_All()
        {
            //Test module configuration
            LocalIocManager.Resolve<IAbpRedisOnlineClientStoreOptions>().DatabaseId.ShouldBe(2);

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
