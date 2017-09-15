using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015092201)]
    public class _20150922_01_Add_Columns_To_AbpAuditLogs : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpAuditLogs")
                .AddColumn("ImpersonatorUserId").AsInt64().Nullable()
                .AddColumn("ImpersonatorTenantId").AsInt32().Nullable()
                .AddColumn("CustomData").AsString(2000).Nullable();
        }
    }
}