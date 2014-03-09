using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20140122
{
    [Migration(2014012201)]
    public class _01_RemoveColumnsFromRoleTable : ForwardOnlyMigration
    {
        public override void Up()
        {
            Delete.Column("IsFrozen").FromTable("AbpRoles");
            Delete.Column("IsStatic").FromTable("AbpRoles");
        }
    }
}
