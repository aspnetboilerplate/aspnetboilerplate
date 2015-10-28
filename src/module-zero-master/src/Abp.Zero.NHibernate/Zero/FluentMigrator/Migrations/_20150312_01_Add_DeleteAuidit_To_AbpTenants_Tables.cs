using Abp.FluentMigrator.Extensions;
using FluentMigrator;

namespace Abp.Zero.FluentMigrator.Migrations
{
    [Migration(2015031201)]
    public class _20150312_01_Add_DeleteAuidit_To_AbpTenants_Tables : AutoReversingMigration
    {
        public override void Up()
        {
            Alter.Table("AbpUsers")
                .AddIsDeletedColumn()
                .AddColumn("DeleterUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id")
                .AddColumn("DeletionTime").AsDateTime().Nullable();
        }
    }
}