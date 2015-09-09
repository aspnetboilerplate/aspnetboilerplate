using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RedisCache.Configuration
{
    public class AbpRedisConnectionProvider : IAbpRedisConnectionProvider
    {
        private static ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> _connectionMultiplexers = new ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();
      


        public string GetConnectionString(string service)
        {
           
            var _defaultSettingsKey = service;

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[_defaultSettingsKey];

            if (connectionStringSettings == null)
            {
                throw new ConfigurationErrorsException("A connection string is expected for " + service);
            }

            return connectionStringSettings.ConnectionString;
        }

        public ConnectionMultiplexer GetConnection(string connectionString)
        {

            if (String.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            // when using ConcurrentDictionary, multiple threads can create the value
            // at the same time, so we need to pass a Lazy so that it's only 
            // the object which is added that will create a ConnectionMultiplexer,
            // even when a delegate is passed

            return _connectionMultiplexers.GetOrAdd(connectionString,
                new Lazy<ConnectionMultiplexer>(() =>
                {
                  
                    return ConnectionMultiplexer.Connect(connectionString);
                })).Value;
        }
    }
}
