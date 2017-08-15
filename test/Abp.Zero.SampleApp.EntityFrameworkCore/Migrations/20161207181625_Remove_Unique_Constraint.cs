using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Abp.Zero.SampleApp.EntityFrameworkCore.Migrations
{
    public partial class Remove_Unique_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_CreatorUserId1",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_DeleterUserId1",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_LastModifierUserId1",
                table: "AbpTenants");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "LastModifierUserId1",
                table: "AbpTenants",
                newName: "UserId2");

            migrationBuilder.RenameColumn(
                name: "DeleterUserId1",
                table: "AbpTenants",
                newName: "UserId1");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId1",
                table: "AbpTenants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_LastModifierUserId1",
                table: "AbpTenants",
                newName: "IX_AbpTenants_UserId2");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_DeleterUserId1",
                table: "AbpTenants",
                newName: "IX_AbpTenants_UserId1");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_CreatorUserId1",
                table: "AbpTenants",
                newName: "IX_AbpTenants_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "EmailConfirmationCode",
                table: "AbpUsers",
                maxLength: 328,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "AbpUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsLockoutEnabled",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneNumberConfirmed",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsTwoFactorEnabled",
                table: "AbpUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockoutEndDateUtc",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AbpUserClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbpUserClaims_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_DeleterUserId",
                table: "AbpUsers",
                column: "DeleterUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers",
                column: "LastModifierUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpUserClaims_UserId",
                table: "AbpUserClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId",
                table: "AbpTenants",
                column: "UserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId1",
                table: "AbpTenants",
                column: "UserId1",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId2",
                table: "AbpTenants",
                column: "UserId2",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpUsers_AbpUsers_DeleterUserId",
                table: "AbpUsers",
                column: "DeleterUserId",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId1",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpTenants_AbpUsers_UserId2",
                table: "AbpTenants");

            migrationBuilder.DropForeignKey(
                name: "FK_AbpUsers_AbpUsers_DeleterUserId",
                table: "AbpUsers");

            migrationBuilder.DropTable(
                name: "AbpUserClaims");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_DeleterUserId",
                table: "AbpUsers");

            migrationBuilder.DropIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsLockoutEnabled",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsPhoneNumberConfirmed",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "IsTwoFactorEnabled",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "LockoutEndDateUtc",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "AbpUsers");

            migrationBuilder.RenameColumn(
                name: "UserId2",
                table: "AbpTenants",
                newName: "LastModifierUserId1");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "AbpTenants",
                newName: "DeleterUserId1");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AbpTenants",
                newName: "CreatorUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_UserId2",
                table: "AbpTenants",
                newName: "IX_AbpTenants_LastModifierUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_UserId1",
                table: "AbpTenants",
                newName: "IX_AbpTenants_DeleterUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_AbpTenants_UserId",
                table: "AbpTenants",
                newName: "IX_AbpTenants_CreatorUserId1");

            migrationBuilder.AlterColumn<string>(
                name: "EmailConfirmationCode",
                table: "AbpUsers",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 328,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_CreatorUserId",
                table: "AbpUsers",
                column: "CreatorUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpUsers_LastModifierUserId",
                table: "AbpUsers",
                column: "LastModifierUserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_CreatorUserId1",
                table: "AbpTenants",
                column: "CreatorUserId1",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_DeleterUserId1",
                table: "AbpTenants",
                column: "DeleterUserId1",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AbpTenants_AbpUsers_LastModifierUserId1",
                table: "AbpTenants",
                column: "LastModifierUserId1",
                principalTable: "AbpUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
