namespace Abp
{
    public class AbpRedisCacheConfig
    {
        private string connectionStringKey = "Abp.Redis.Cache";

        public string ConnectionStringKey
        {
            get { return connectionStringKey; }
            set { connectionStringKey = value; }
        }
    }
}
