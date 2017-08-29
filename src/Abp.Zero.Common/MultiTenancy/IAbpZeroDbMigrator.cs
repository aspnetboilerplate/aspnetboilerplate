namespace Abp.MultiTenancy
{
    public interface IAbpZeroDbMigrator
    {
        void CreateOrMigrateForHost();

        void CreateOrMigrateForTenant(AbpTenantBase tenant);
    }
}
