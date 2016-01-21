using Abp.Dependency;

namespace Abp
{
    public class AbpRedisCacheConfig : ISingletonDependency
    {
        private string connectionStringKey = "Abp.Redis.Cache";

        public string ConnectionStringKey
        {
            get { return connectionStringKey; }
            set { connectionStringKey = value; }
        }
    }
}
