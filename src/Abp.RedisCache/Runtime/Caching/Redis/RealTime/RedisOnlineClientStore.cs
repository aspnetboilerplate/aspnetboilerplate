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
        var userIdentifier = client.ToUserIdentifierOrNull();

        const string script = @"
            redis.call('SADD', KEYS[1], ARGV[1]);
            redis.call('HSET', KEYS[2], ARGV[1], ARGV[2]);
            return 1;";

        if (userIdentifier != null)
        {
            var userConnectionsKey = GetUserConnectionsKey(userIdentifier);
            await database.ScriptEvaluateAsync(script,
                new RedisKey[] { userConnectionsKey, _clientStoreKey },
                new RedisValue[] { client.ConnectionId, client.ToJsonString() });
        }
        else
        {
            await database.HashSetAsync(_clientStoreKey, new[]
            {
                new HashEntry(client.ConnectionId, client.ToJsonString())
            });
        }
    }

    public async Task<bool> RemoveAsync(string connectionId)
    {
        var (success, _) = await TryRemoveInternalAsync(connectionId);
        return success;
    }

    public async Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
        var (success, client) = await TryRemoveInternalAsync(connectionId);
        clientAction?.Invoke(client);
        return success;
    }

    public async Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
        var database = GetDatabase();
        var clientValue = await database.HashGetAsync(_clientStoreKey, connectionId);
        if (clientValue.IsNullOrEmpty)
        {
            clientAction?.Invoke(null);
            return false;
        }

        var onlineClient = JsonSerializer.Deserialize<OnlineClient>(clientValue.ToString());
        clientAction?.Invoke(onlineClient);
        return true;
    }

    public async Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
    {
        var database = GetDatabase();
        var clientsEntries = await database.HashGetAllAsync(_clientStoreKey);
        return clientsEntries
            .Select(entry => JsonSerializer.Deserialize<OnlineClient>(entry.Value))
            .Cast<IOnlineClient>()
            .ToImmutableList();
    }


    public async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
    {
        var database = GetDatabase();
        var userConnectionsKey = GetUserConnectionsKey(userIdentifier);

        var connectionIdValues = await database.SetMembersAsync(userConnectionsKey);
        if (connectionIdValues.Length == 0)
        {
            return ImmutableList<IOnlineClient>.Empty;
        }

        var clientValues = await database.HashGetAsync(_clientStoreKey, connectionIdValues);

        return clientValues
            .Where(clientValue => !clientValue.IsNullOrEmpty)
            .Select(clientValue => JsonSerializer.Deserialize<OnlineClient>(clientValue.ToString()))
            .Cast<IOnlineClient>()
            .ToImmutableList();
    }

    private IDatabase GetDatabase()
    {
        return _database.GetDatabase();
    }

    private string GetUserConnectionsKey(UserIdentifier userIdentifier)
    {
        return $"{_userStoreKey}:{userIdentifier.ToUserIdentifierString()}";
    }


    private async Task<(bool Success, IOnlineClient Client)> TryRemoveInternalAsync(string connectionId)
    {
        var database = GetDatabase();

        var clientJson = await database.HashGetAsync(_clientStoreKey, connectionId);
        if (clientJson.IsNullOrEmpty)
        {
            return (false, null);
        }

        var client = JsonSerializer.Deserialize<OnlineClient>(clientJson.ToString());
        var userIdentifier = client.ToUserIdentifierOrNull();

        var userConnectionsKey = userIdentifier != null
            ? GetUserConnectionsKey(userIdentifier)
            : null;

        const string script = @"
            -- KEYS[1] = _clientStoreKey
            -- KEYS[2] = userConnectionsKey
            -- ARGV[1] = connectionId

            redis.call('HDEL', KEYS[1], ARGV[1]);
        
            if KEYS[2] then
                redis.call('SREM', KEYS[2], ARGV[1]);
            end

            return ARGV[2];
            ";

        var keys = new List<RedisKey> { _clientStoreKey };
        if (userConnectionsKey != null)
        {
            keys.Add(userConnectionsKey);
        }

        await database.ScriptEvaluateAsync(script, keys.ToArray(), new RedisValue[] { connectionId, clientJson });

        return (true, client);
    }
}