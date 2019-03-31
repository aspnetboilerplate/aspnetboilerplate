namespace Abp.RealTime.Redis
{
    public interface IAbpRedisOnlineClientStoreOptions
    {
        string ConnectionString { get; set; }
        string StoreName { get; set; }
        int DatabaseId { get; set; }
    }
}