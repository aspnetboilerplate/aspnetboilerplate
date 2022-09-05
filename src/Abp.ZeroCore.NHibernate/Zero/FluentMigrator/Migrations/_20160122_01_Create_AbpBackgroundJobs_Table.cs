using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2016012201)]
    public class _20160122_01_Create_AbpBackgroundJobs_Table : AutoReversingMigration
    {
        public override void Up()
        {
            Create.Table("AbpBackgroundJobs")
                .WithIdAsInt32()
                .WithColumn("JobType").AsString(512).NotNullable()
                .WithColumn("JobArgs").AsString(1048576).NotNullable()
                .WithColumn("TryCount").AsInt16().NotNullable().WithDefaultValue(0)
                .WithColumn("NextTryTime").AsDateTime().NotNullable()
                .WithColumn("LastTryTime").AsDateTime().Nullable()
                .WithColumn("IsAbandoned").AsBoolean().Nullable().WithDefaultValue(false)
                .WithColumn("Priority").AsByte().NotNullable().WithDefaultValue(15)
                .WithCreationAuditColumns();

            Create.Index("IX_IsAbandoned_NextTryTime")
                .OnTable("AbpBackgroundJobs")
                .OnColumn("IsAbandoned").Ascending()
                .OnColumn("NextTryTime").Ascending()
                .WithOptions().NonClustered();
        }
    }
}
