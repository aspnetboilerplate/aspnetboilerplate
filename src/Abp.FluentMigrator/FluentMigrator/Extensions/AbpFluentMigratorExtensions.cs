using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using FluentMigrator;
using FluentMigrator.Builders.Alter.Table;
using FluentMigrator.Builders.Create.Table;

namespace Abp.FluentMigrator.Extensions
{
    /// <summary>
    /// This class is an extension for migration system to make easier to some common tasks.
    /// </summary>
    public static class AbpFluentMigratorExtensions
    {
        /// <summary>
        /// Adds an auto increment <see cref="int"/> primary key to the table.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt32(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity();
        }

        /// <summary>
        /// Adds an auto increment <see cref="long"/> primary key to the table.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIdAsInt64(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("Id").AsInt64().NotNullable().PrimaryKey().Identity();
        }

        /// <summary>
        /// Adds IsDeleted column to the table. See <see cref="ISoftDelete"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIsDeletedColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        /// <summary>
        /// Adds IsDeleted column to the table. See <see cref="ISoftDelete"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddIsDeletedColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false);
        }

        /// <summary>
        /// Adds DeletionTime column to a table. See <see cref="IDeletionAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithDeletionTimeColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("DeletionTime").AsDateTime().Nullable();
        }

        /// <summary>
        /// Adds DeletionTime column to a table. See <see cref="IDeletionAudited"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddDeletionTimeColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("DeletionTime").AsDateTime().Nullable();
        }

        /// <summary>
        /// Ads CreationTime field to the table for <see cref="IHasCreationTime"/> interface.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithCreationTimeColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        /// <summary>
        /// Adds CreationTime field to a table. See <see cref="IHasCreationTime"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddCreationTimeColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        /// <summary>
        /// Adds LastModificationTime field to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddLastModificationTimeColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("LastModificationTime").AsDateTime().Nullable();
        }

        /// <summary>
        /// Adds LastModificationTime field to a table. See <see cref="IModificationAudited"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithLastModificationTimeColumn(this ICreateTableWithColumnSyntax table, bool defaultValue = true)
        {
            return table
                .WithColumn("LastModificationTime").AsDateTime().Nullable();
        }

        /// <summary>
        /// Adds IsDeleted column to the table. See <see cref="IPassivable"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithIsActiveColumn(this ICreateTableWithColumnSyntax table, bool defaultValue = true)
        {
            return table
                .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(defaultValue);
        }

        /// <summary>
        /// Adds IsDeleted column to the table. See <see cref="IPassivable"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddIsActiveColumn(this IAlterTableAddColumnOrAlterColumnSyntax table, bool defaultValue = true)
        {
            return table
                .AddColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(defaultValue);
        }
    }
}
