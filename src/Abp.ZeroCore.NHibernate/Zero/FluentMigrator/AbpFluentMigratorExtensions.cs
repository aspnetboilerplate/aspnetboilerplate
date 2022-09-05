using Abp.Domain.Entities.Auditing;
using Abp.FluentMigrator.Extensions;
using Abp.MultiTenancy;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;

namespace Abp.Zero.FluentMigrator
{
    /// <summary>
    /// This class is an extension for migration system to make easier to some common tasks.
    /// </summary>
    public static class AbpZeroFluentMigratorExtensions
    {
        #region Create table

        /// <summary>
        /// Adds full auditing fields to a table. See <see cref="IFullAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithFullAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithAuditColumns()
                .WithDeletionAuditColumns();
        }

        /// <summary>
        /// Adds auditing fields to a table. See <see cref="IAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithCreationAuditColumns()
                .WithModificationAuditColumns();
        }

        /// <summary>
        /// Adds CreatorUserId field to a table. See <see cref="ICreationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithCreatorUserIdColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("CreatorUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        /// <summary>
        /// Adds creation auditing fields to a table. See <see cref="ICreationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithCreationAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithCreationTimeColumn()
                .WithCreatorUserIdColumn();
        }

        /// <summary>
        /// Adds LastModifierUserId field to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithLastModifierUserIdColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("LastModifierUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        /// <summary>
        /// Adds modification auditing fields to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithModificationAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithLastModificationTimeColumn()
                .WithLastModifierUserIdColumn();
        }

        /// <summary>
        /// Adds DeleterUserId field to a table. See <see cref="IDeletionAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithDeleterUserIdColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("DeleterUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        /// <summary>
        /// Adds deletion auditing columns to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithDeletionAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithIsDeletedColumn()
                .WithDeletionTimeColumn()
                .WithDeleterUserIdColumn();
        }

        /// <summary>
        /// Adds TenantId column to a table as not nullable. See <see cref="AbpTenant{TUser}"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithTenantIdAsRequired(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id");
        }

        /// <summary>
        /// Adds TenantId column to a table as nullable. See <see cref="AbpTenant{TUser}"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithTenantIdAsNullable(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("TenantId").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");
        }

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithUserId(this ICreateTableWithColumnSyntax table)
        {
            return table.WithUserId("UserId");
        }

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithNullableUserId(this ICreateTableWithColumnSyntax table)
        {
            return table.WithNullableUserId("UserId");
        }

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithUserId(this ICreateTableWithColumnSyntax table, string columnName)
        {
            return table
                .WithColumn(columnName).AsInt64().NotNullable().ForeignKey("AbpUsers", "Id");
        }

        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithNullableUserId(this ICreateTableWithColumnSyntax table, string columnName)
        {
            return table
                .WithColumn(columnName).AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        #endregion

        #region Alter table

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddFullAuditColumns(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddAuditColumns()
                .AddDeletionAuditColumns();
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddAuditColumns(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddCreationAuditColumns()
                .AddModificationAuditColumns();
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddCreatorUserIdColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("CreatorUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddCreationAuditColumns(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddCreationTimeColumn()
                .AddCreatorUserIdColumn();
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddLastModifierUserIdColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("LastModifierUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddModificationAuditColumns(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddLastModificationTimeColumn()
                .AddLastModifierUserIdColumn();
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddDeletionAuditColumns(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddIsDeletedColumn()
                .AddDeletionTimeColumn()
                .AddColumn("DeleterUserId").AsInt64().Nullable().ForeignKey("AbpUsers", "Id");
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddTenantIdColumnAsRequired(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id");
        }

        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddTenantIdColumnAsNullable(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("TenantId").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");
        }

        #endregion
    }
}