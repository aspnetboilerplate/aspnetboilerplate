namespace Abp.MultiTenancy
{
    public class TenantInfo
    {
        public long Id { get; set; }

        public string TenancyName { get; set; }

        public TenantInfo(long id, string tenancyName)
        {
            Id = id;
            TenancyName = tenancyName;
        }
    }
}