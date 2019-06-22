using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Extensions;
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
            var fileName = Assembly.GetExecutingAssembly()
                .GetManifestResourceNames().FirstOrDefault(x =>
                    x.Contains(name) && x.EndsWith(".sql", StringComparison.CurrentCultureIgnoreCase));

            if (!fileName.IsNullOrWhiteSpace())
            {
                using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                {
                    if (resource != null)
                    {
                        using (var sr = new StreamReader(resource))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}
