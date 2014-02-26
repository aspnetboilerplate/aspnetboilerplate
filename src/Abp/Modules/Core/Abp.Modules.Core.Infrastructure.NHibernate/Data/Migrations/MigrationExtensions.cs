using Abp.Domain.Entities.Auditing;
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace Abp.Modules.Core.Data.Migrations
{
    /// <summary>
    /// This class is an extension for migration system to make easier to some common tasks.
    /// </summary>
    public static class MigrationExtensions
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
                .WithColumn("CreationTime").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentDateTime)
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
    }
}
