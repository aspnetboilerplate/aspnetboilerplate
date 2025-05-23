using Abp.Dependency;
using Abp.RealTime;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Immutable;
using System.Text.Json;
using Abp.Json;

namespace Abp.Runtime.Caching.Redis.RealTime;

public class RedisOnlineClientStore : IOnlineClientStore, ISingletonDependency
{
    private readonly IAbpRedisCacheDatabaseProvider _database;

    private readonly string _clientStoreKey;
    private readonly string _userStoreKey;

    public RedisOnlineClientStore(
        IAbpRedisCacheDatabaseProvider database,
        AbpRedisCacheOptions options)
    {
        _database = database;

        _clientStoreKey = options.OnlineClientsStoreKey + ".Clients";
        _userStoreKey = options.OnlineClientsStoreKey + ".Users";
    }

    public async Task AddAsync(IOnlineClient client)
    {
        var database = GetDatabase();

        // Store a lookup of userId's to connection ID's
        var userIdentifier = client.ToUserIdentifierOrNull();
        if (userIdentifier != null)
        {
            var userIdentifierString = userIdentifier.ToUserIdentifierString();

            var existingUserValue = await database.HashGetAsync(_userStoreKey, userIdentifierString);
            if (existingUserValue.IsNullOrEmpty)
            {
                await database.HashSetAsync(_userStoreKey,
                [
                    new HashEntry(userIdentifierString, new HashSet<string> {client.ConnectionId}.ToJsonString())
                ]);
            }
            else
            {
                var connectionIds = JsonSerializer.Deserialize<HashSet<string>>(existingUserValue);
                connectionIds.Add(client.ConnectionId);
                await database.HashSetAsync(_userStoreKey,
                [
                    new HashEntry(userIdentifierString, connectionIds.ToJsonString())
                ]);
            }
        }

        // Store a lookup of connection ID's to OnlineClient objects
        await database.HashSetAsync(_clientStoreKey,
        [
            new HashEntry(client.ConnectionId, client.ToJsonString())
        ]);
    }

    public async Task<bool> RemoveAsync(string connectionId)
    {
        var database = GetDatabase();

        var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
        if (clientValue.IsNullOrEmpty)
        {
            return true;
        }

        var onlineClient = JsonSerializer.Deserialize<OnlineClient>(clientValue);

        var userIdentifier = onlineClient.ToUserIdentifierOrNull();
        if (userIdentifier != null)
        {
            var userIdentifierString = userIdentifier.ToUserIdentifierString();

            var existingUserValue = await database.HashGetAsync(_userStoreKey, userIdentifierString);
            if (!existingUserValue.IsNullOrEmpty)
            {
                var connectionIds = JsonSerializer.Deserialize<HashSet<string>>(existingUserValue);
                connectionIds.Remove(connectionId);
                if (connectionIds.Count > 0)
                {
                    await database.HashSetAsync(_userStoreKey,
                    [
                        new HashEntry(userIdentifierString, connectionIds.ToJsonString())
                    ]);
                }
                else
                {
                    await database.HashDeleteAsync(_userStoreKey, userIdentifierString);
                }
            }
        }

        await database.HashDeleteAsync(_clientStoreKey, connectionId);
        return true;
    }

    public async Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
        try
        {
            var database = GetDatabase();

            var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
            if (clientValue.IsNullOrEmpty)
            {
                clientAction(null);
                return true;
            }

            var onlineClient = JsonSerializer.Deserialize<OnlineClient>(clientValue);
            clientAction(onlineClient);

            var userIdentifier = onlineClient.ToUserIdentifierOrNull();
            if (userIdentifier != null)
            {
                var userIdentifierString = userIdentifier.ToUserIdentifierString();

                var existingUserValue = await database.HashGetAsync(_userStoreKey, userIdentifierString);
                if (!existingUserValue.IsNullOrEmpty)
                {
                    var connectionIds = JsonSerializer.Deserialize<HashSet<string>>(existingUserValue);
                    connectionIds.Remove(connectionId);
                    if (connectionIds.Count > 0)
                    {
                        await database.HashSetAsync(_userStoreKey,
                        [
                            new HashEntry(userIdentifierString, connectionIds.ToJsonString())
                        ]);
                    }
                    else
                    {
                        await database.HashDeleteAsync(_userStoreKey, userIdentifierString);
                    }
                }
            }

            await database.HashDeleteAsync(_clientStoreKey, connectionId);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            clientAction(null);
            return false;
        }
    }

    public async Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
        var database = GetDatabase();
        var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
        if (clientValue.IsNullOrEmpty)
        {
            clientAction(null);
            return false;
        }

        clientAction(JsonSerializer.Deserialize<OnlineClient>(clientValue));
        return true;
    }

    public async Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
    {
        var database = GetDatabase();
        var clientsEntries = await database.HashGetAllAsync(_clientStoreKey);
        var clients = clientsEntries
            .Select(entry => JsonSerializer.Deserialize<OnlineClient>(entry.Value))
            .Cast<IOnlineClient>()
            .ToList();

        return clients.ToImmutableList();
    }

    public async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
    {
        var database = GetDatabase();

        var userClientsValue = await database.HashGetAsync(_userStoreKey, userIdentifier.ToUserIdentifierString());
        if (!userClientsValue.HasValue)
        {
            return ImmutableList<IOnlineClient>.Empty;
        }

        var connectionIds = JsonSerializer.Deserialize<HashSet<string>>(userClientsValue);
        if (connectionIds == null || !connectionIds.Any())
        {
            return ImmutableList<IOnlineClient>.Empty;
        }

        var redisKeys = connectionIds.Select(connectionId => (RedisValue)connectionId).ToArray();

        var clientValues = await database.HashGetAsync(_clientStoreKey, redisKeys);

        var clients = clientValues
            .Where(clientValue => !clientValue.IsNullOrEmpty)
            .Select(clientValue => JsonSerializer.Deserialize<OnlineClient>(clientValue))
            .Cast<IOnlineClient>()
            .ToImmutableList();

        return clients;
    }

    private IDatabase GetDatabase()
    {
        return _database.GetDatabase();
    }
}