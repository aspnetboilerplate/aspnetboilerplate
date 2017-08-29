using System.Collections.Generic;
using System.Reflection;
using System.Transactions;

using Abp.EntityFramework;
using Abp.Modules;
using Abp.TestBase;

using DapperExtensions.Sql;

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
            Configuration.UnitOfWork.IsolationLevel = IsolationLevel.Unspecified;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            DapperExtensions.DapperExtensions.SqlDialect = new SqliteDialect();

            DapperExtensions.DapperExtensions.SetMappingAssemblies(new List<Assembly> { Assembly.GetExecutingAssembly() });
        }
    }
}
