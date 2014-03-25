using Abp.Security.Tenants;
using FluentMigrator.Builders.Create.Table;

namespace Abp.Data.Migrations.FluentMigrator
{
    /// <summary>
    /// This class is an extension for migration system to make easier to some common tasks.
    /// </summary>
    public static class AbpCoreModuleFluentMigratorExtensions
    {
        /// <summary>
        /// Adds TenantId column to a table as not nullable. See <see cref="AbpTenant"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithTenantId(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("TenantId").AsInt32().NotNullable().ForeignKey("AbpTenants", "Id");
        }

        /// <summary>
        /// Adds TenantId column to a table as nullable. See <see cref="AbpTenant"/>.
        /// </summary>
        public static ICreateTableColumnOptionOrForeignKeyCascadeOrWithColumnSyntax WithNullableTenantId(this ICreateTableWithColumnSyntax table)
        {
            return table
                .WithColumn("TenantId").AsInt32().Nullable().ForeignKey("AbpTenants", "Id");
        }
    }
}
