using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.EntityFramework;
using Abp.EntityFramework.Uow;
using Abp.Modules;
using Abp.TestBase;

using Dapper;

namespace Abp.Dapper.Tests
{
    [DependsOn(
        typeof(AbpEntityFrameworkModule),
        typeof(AbpTestBaseModule),
        typeof(AbpDapperModule)
    )]
    public class AbpDapperTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService<IEfTransactionStrategy, DbContextEfTransactionStrategy>(DependencyLifeStyle.Transient);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { Assembly.GetExecutingAssembly() });
        }

        public override void Shutdown()
        {
            var connection = new SqlConnection(Configuration.DefaultNameOrConnectionString);

            var files = new List<string>
            {
                ReadScriptFile("DestroyScript")
            };

            foreach (string setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        private string ReadScriptFile(string name)
        {
            string fileName = GetType().Namespace + ".Scripts" + "." + name + ".sql";
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

            return string.Empty;
        }
    }
}
