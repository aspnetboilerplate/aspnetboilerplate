using Abp.RealTime;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xunit;

namespace Abp.Tests.RealTime
{
    public class InMemoryOnlineClientManager_Tests
    {
        private readonly IOnlineClientManager _clientManager;
        private static readonly RandomNumberGenerator KeyGenerator = RandomNumberGenerator.Create();

        public InMemoryOnlineClientManager_Tests()
        {
            
            _clientManager = new OnlineClientManager(new InMemoryOnlineClientStore());
        }

        [Fact]
        public async Task Test_All()
        {
            int tenantId = 1;

            Dictionary<string, int> connections = new Dictionary<string, int>();

            for (int i = 0; i < 100; i++)
            {
                connections.Add(MakeNewConnectionId(), i + 1);
            }

            foreach (var pair in connections)
            {
                await _clientManager.AddAsync(new OnlineClient(pair.Key, "127.0.0.1", tenantId, pair.Value));
            }

            var testId = connections.Keys.ToList()[5];

            (await _clientManager.GetAllClientsAsync()).Count.ShouldBe(connections.Count);
            (await _clientManager.GetAllByUserIdAsync(new UserIdentifier(tenantId, connections[testId]))).Count.ShouldBe(1);
            (await _clientManager.GetByConnectionIdOrNullAsync(testId)).ShouldNotBeNull();
            (await _clientManager.RemoveAsync(testId)).ShouldBeTrue();
            (await _clientManager.GetAllClientsAsync()).Count.ShouldBe(connections.Count - 1);
            (await _clientManager.GetByConnectionIdOrNullAsync(testId)).ShouldBeNull();
            (await _clientManager.GetAllByUserIdAsync(new UserIdentifier(tenantId, connections[testId]))).Count.ShouldBe(0);
        }

        private static string MakeNewConnectionId()
        {
            var buffer = new byte[16];
            KeyGenerator.GetBytes(buffer);
            return Convert.ToBase64String(buffer);
        }
    }
}
