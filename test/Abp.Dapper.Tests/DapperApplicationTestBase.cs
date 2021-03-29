using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

using Abp.Configuration.Startup;
using Abp.TestBase;

using Castle.MicroKernel.Registration;

using Dapper;

namespace Abp.Dapper.Tests
{
    public abstract class DapperApplicationTestBase : AbpIntegratedTestBase<AbpDapperTestModule>
    {
        protected DapperApplicationTestBase()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString = "Data Source=:memory:";

            AbpSession.UserId = 1;
            AbpSession.TenantId = 1;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                         .UsingFactoryMethod(() =>
                         {
                             var connection = new SQLiteConnection(Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString);
                             connection.Open();
                             var files = new List<string>
                             {
                                 ReadScriptFile("CreateInitialTables")
                             };

                             foreach (string setupFile in files)
                             {
                                 connection.Execute(setupFile);
                             }

                             return connection;
                         })
                         .LifestyleSingleton()
            );
        }

        private string ReadScriptFile(string name)
        {
            var fileName = GetType().Namespace + ".Scripts" + "." + name + ".sql";
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                if (resource == null)
                {
                    return string.Empty;
                }
                
                using (var sr = new StreamReader(resource))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
