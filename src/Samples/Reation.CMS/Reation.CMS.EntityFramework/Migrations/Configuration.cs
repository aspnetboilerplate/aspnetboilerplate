using EntityFramework.DynamicFilters;
using Reation.CMS.Migrations.Data;
using System.Data.Entity.Migrations;

namespace Reation.CMS.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<CMS.EntityFramework.CMSDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CMS";
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new MySqlHistoryContext(conn, schema));
            CodeGenerator = new MySql.Data.Entity.MySqlMigrationCodeGenerator();
        }

        protected override void Seed(CMS.EntityFramework.CMSDbContext context)
        {
            context.DisableAllFilters();
            new InitialDataBuilder().Build(context);
            // This method will be called every time after migrating to the latest version.
            // You can add any seed data here...
        }
    }
}
