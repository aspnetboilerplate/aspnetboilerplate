using Adorable.Dependency;

namespace Adorable
{
    public class AbpRedisCacheConfig : ISingletonDependency
    {
        private string connectionStringKey = "Adorable.Redis.Cache";

        public string ConnectionStringKey
        {
            get { return connectionStringKey; }
            set { connectionStringKey = value; }
        }

        private string databaseIdAppSetting = "Adorable.Redis.Cache.DatabaseId";

        public string DatabaseIdAppSetting
        {
            get { return databaseIdAppSetting; }
            set { databaseIdAppSetting = value; }
        }
    }
}
