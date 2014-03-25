using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Abp.Data.Migrations.FluentMigrator
{
    /// <summary>
    /// This class is an extension for migration system to make easier to some common tasks.
    /// </summary>
    public static class AbpFluentMigratorExtensions
    {
        /// <summary>
        /// Adds auditing columns to a table. See <see cref="IAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithCreationAuditColumns()
                .WithModificationAuditColumns();
        }

        /// <summary>
        /// Adds creation auditing columns to a table. See <see cref="ICreationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithCreationAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithCreationTimeColumn()
                .WithColumn("CreatorUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id");
        }

        /// <summary>
        /// Adds modification auditing columns to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithModificationAuditColumns(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("LastModificationTime").AsDateTime().Nullable()
                .WithColumn("LastModifierUserId").AsInt32().Nullable().ForeignKey("AbpUsers", "Id");
        }

        /// <summary>
        /// Adds creation auditing columns to a table. See <see cref="ICreationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithCreationTimeColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        /// <summary>
        /// Adds an auto increment <see cref="int"/> primary key to the table.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt32(this ICreateTableWithColumnSyntax table)
        {
            return table.WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity();
        }

        /// <summary>
        /// Adds an auto increment <see cref="long"/> primary key to the table.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt64(this ICreateTableWithColumnSyntax table)
        {
            return table.WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity();
        }

        /// <summary>
        /// Adds IsDeleted column to the table. See <see cref="ISoftDeleteEntity"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIsDeletedColumn(this ICreateTableWithColumnSyntax table)
        {
            return table.WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
        }
    }
}
