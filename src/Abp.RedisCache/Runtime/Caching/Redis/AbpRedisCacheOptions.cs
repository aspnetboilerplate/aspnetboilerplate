#if NET46
using System.Configuration;
#endif
using Abp.Configuration.Startup;
using Abp.Extensions;

namespace Abp.Runtime.Caching.Redis
{
    public class AbpRedisCacheOptions
    {
        public IAbpStartupConfiguration AbpStartupConfiguration { get; private set; }

        private const string ConnectionStringKey = "Abp.Redis.Cache";

        private const string DatabaseIdSettingKey = "Abp.Redis.Cache.DatabaseId";

        public string ConnectionString { get; set; }

        public int DatabaseId { get; set; }

        public AbpRedisCacheOptions(IAbpStartupConfiguration abpStartupConfiguration)
        {
            AbpStartupConfiguration = abpStartupConfiguration;

            ConnectionString = GetDefaultConnectionString();
            DatabaseId = GetDefaultDatabaseId();
        }

        private static int GetDefaultDatabaseId()
        {
#if NET46
            var appSetting = ConfigurationManager.AppSettings[DatabaseIdSettingKey];
            if (appSetting.IsNullOrEmpty())
            {
                return -1;
            }

            int databaseId;
            if (!int.TryParse(appSetting, out databaseId))
            {
                return -1;
            }

            return databaseId;
#else
            return -1;
#endif
        }

        private static string GetDefaultConnectionString()
        {
#if NET46
            var connStr = ConfigurationManager.ConnectionStrings[ConnectionStringKey];
            if (connStr == null || connStr.ConnectionString.IsNullOrWhiteSpace())
            {
                return "localhost";
            }

            return connStr.ConnectionString;
#else
            return "localhost";
#endif
        }
    }
}