using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015123002)]
    public class _20151230_02_Create_Indexes_For_AuditLogs : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Index("IX_TenantId_ExecutionTime")
                .OnTable("AbpAuditLogs")
                .OnColumn("TenantId").Ascending()
                .OnColumn("ExecutionTime").Ascending()
                .WithOptions().NonClustered();

            Create.Index("IX_UserId_ExecutionTime")
                .OnTable("AbpAuditLogs")
                .OnColumn("UserId").Ascending()
                .OnColumn("ExecutionTime").Ascending()
                .WithOptions().NonClustered();
        }
    }
}