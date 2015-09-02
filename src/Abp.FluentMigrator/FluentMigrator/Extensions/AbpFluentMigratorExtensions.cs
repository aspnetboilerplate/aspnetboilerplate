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
        /// Ads CreationTime field to the table for <see cref="IHasCreationTime"/> interface.
        /// </summary>
        public static ICreateTableColumnOptionOrWithColumnSyntax WithCreationTimeColumn(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
        }

        /// <summary>
        /// Adds creation auditing columns to a table. See <see cref="ICreationAudited"/>.
        /// </summary>
        public static IAlterTableColumnOptionOrAddColumnOrAlterColumnSyntax AddCreationTimeColumn(this IAlterTableAddColumnOrAlterColumnSyntax table)
        {
            return table
                .AddColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime);
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
