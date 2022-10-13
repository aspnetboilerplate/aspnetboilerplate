using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015081102)]
    public class _20150811_02_CreateAbpAuditLogsTable : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpAuditLogs")
                .WithIdAsInt64()
                .WithTenantIdAsNullable()
                .WithNullableUserId()
                .WithColumn("ServiceName").AsString(256).Nullable()
                .WithColumn("MethodName").AsString(256).Nullable()
                .WithColumn("Parameters").AsString(1024).Nullable()
                .WithColumn("ExecutionTime").AsDateTime().NotNullable()
                .WithColumn("ExecutionDuration").AsInt32().NotNullable()
                .WithColumn("ClientIpAddress").AsString(64).Nullable()
                .WithColumn("ClientName").AsString(128).Nullable()
                .WithColumn("BrowserInfo").AsString(256).Nullable()
                .WithColumn("Exception").AsString(2000).Nullable();
        }
    }
}