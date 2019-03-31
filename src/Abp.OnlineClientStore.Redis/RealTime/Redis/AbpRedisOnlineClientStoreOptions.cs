using Abp.Configuration.Startup;

namespace Abp.RealTime.Redis
{
    public class AbpRedisOnlineClientStoreOptions : IAbpRedisOnlineClientStoreOptions
    {
        public IAbpStartupConfiguration AbpStartupConfiguration { get; }

        private const string StoreNameKey = "Abp.OnlineClientStore.Redis";

        public string ConnectionString { get; set; }

        public string StoreName { get; set; }

        public int DatabaseId { get; set; }

        public AbpRedisOnlineClientStoreOptions(IAbpStartupConfiguration abpStartupConfiguration)
        {
            AbpStartupConfiguration = abpStartupConfiguration;
            StoreName = StoreNameKey;
            ConnectionString = "localhost";
            DatabaseId = -1;
        }
    }
}