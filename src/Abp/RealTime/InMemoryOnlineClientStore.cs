using Abp.Dependency;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Abp.RealTime
{
    public class InMemoryOnlineClientStore : IOnlineClientStore, ISingletonDependency
    {
        /// <summary>
        /// Online clients.
        /// </summary>
        protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

        public InMemoryOnlineClientStore()
        {
            Clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public Task AddAsync(IOnlineClient client)
        {
            Clients.AddOrUpdate(client.ConnectionId, client, (s, o) => client);
            return Task.CompletedTask;
        }

        public Task<bool> RemoveAsync(string connectionId)
        {
            return TryRemoveAsync(connectionId, value => _ = value);
        }

        public Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            var hasRemoved = Clients.TryRemove(connectionId, out IOnlineClient client);
            clientAction(client);
            return Task.FromResult(hasRemoved);
        }

        public Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
        {
            var hasValue = Clients.TryGetValue(connectionId, out IOnlineClient client);
            clientAction(client);
            return Task.FromResult(hasValue);
        }

        public Task<bool> ContainsAsync(string connectionId)
        {
            var hasKey = Clients.ContainsKey(connectionId);
            return Task.FromResult(hasKey);
        }

        public Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
        {
            return Task.FromResult<IReadOnlyList<IOnlineClient>>(Clients.Values.ToImmutableList());
        }

        public async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
        {
            return (await GetAllAsync())
                .Where(c => c.UserId == userIdentifier.UserId && c.TenantId == userIdentifier.TenantId)
                .ToImmutableList();
        }
    }
}
