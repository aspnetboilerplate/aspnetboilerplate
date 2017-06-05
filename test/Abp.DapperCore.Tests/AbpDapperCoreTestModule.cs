using System.Collections.Generic;
using System.Reflection;
using Abp.Modules;
using Abp.TestBase;
using Abp.EntityFrameworkCore;
using Abp.Reflection.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using Castle.MicroKernel.Registration;
using System;
using System.Data.Common;

namespace Abp.DapperCore.Tests
{
    [DependsOn(
        typeof(AbpEntityFrameworkCoreModule),
        typeof(AbpTestBaseModule),
        typeof(AbpDapperCoreModule)
    )]
    public class AbpDapperCoreTestModule : AbpModule
    {
        private Assembly _thisAssembly;
        private readonly string _connectionString;

        public AbpDapperCoreTestModule()
        {
            _thisAssembly = typeof(AbpDapperCoreTestModule).GetAssembly();

            string executable = AppContext.BaseDirectory;
            string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(executable))) + @"\Db\AbpDapperTest.mdf";
            _connectionString = $@"Data Source=(localdb)\MsSqlLocalDb;Integrated Security=True;AttachDbFilename={path}";
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(_thisAssembly);

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { _thisAssembly });

            // Setup database
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            var files = new List<string>
            {
                ReadScriptFile("CreateInitialTables")
            };

            foreach (string setupFile in files)
            {
                connection.Execute(setupFile);
            }
        }

        public override void PreInitialize()
        {
            // Setup database
            var connection = new SqlConnection(_connectionString);

            // SampleDapperApplicationDbContext
            var blogDbOptionsBuilder = new DbContextOptionsBuilder<SampleDapperApplicationDbContext>();
            blogDbOptionsBuilder.UseSqlServer(connection);
            
            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SampleDapperApplicationDbContext>>()
                    .Instance(blogDbOptionsBuilder.Options)
                    .LifestyleSingleton()
            );

            // SampleApplicationDbContext2
            var supportDbOptionsBuilder = new DbContextOptionsBuilder<SampleApplicationDbContext2>();
            supportDbOptionsBuilder.UseSqlServer(connection);

            IocManager.IocContainer.Register(
                Component
                    .For<DbContextOptions<SampleApplicationDbContext2>>()
                    .Instance(supportDbOptionsBuilder.Options)
                    .LifestyleSingleton()
            );
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
            using (Stream resource = _thisAssembly.GetManifestResourceStream(fileName))
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
