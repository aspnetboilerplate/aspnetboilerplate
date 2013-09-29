using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Entities;
using FluentMigrator;

namespace Abp.Modules.Core.Data.Migrations.V20130824
{
    [Migration(2013082404)]
    public class _04_CreateAbpTenantUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("AbpTenantUsers")
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id")
                .WithColumn("UserId").AsInt32().NotNullable().ForeignKey("AbpUsers", "Id")
                .WithColumn("MembershipStatus").AsByte().NotNullable().WithDefaultValue((byte) TenantMembershipStatus.WaitingForApproval)
                .WithColumn("ApprovedUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id")
                .WithCreationAuditColumns();

            Insert.IntoTable("AbpTenantUsers").Row(
                new
                    {
                        TenantId = 1,
                        UserId = 1,
                        MembershipStatus = (byte)TenantMembershipStatus.Member
                    }
                );
        }

        public override void Down()
        {
            Delete.Table("AbpTenantUsers");
        }
    }
}