using Abp.Configuration.Startup;
using Abp.TestBase;
using Abp.Timing;
using System.Reflection;
using Abp.Reflection.Extensions;
using System;
using System.IO;

namespace Abp.DapperCore.Tests
{
    public abstract class DapperCoreApplicationTestBase : AbpIntegratedTestBase<AbpDapperCoreTestModule>
    {
        private readonly Assembly _thisAssembly;
        private readonly string _connectionString;

        protected DapperCoreApplicationTestBase()
        {
            _thisAssembly = typeof(AbpDapperCoreTestModule).GetAssembly();

            Clock.Provider = ClockProviders.Utc;

            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            string executable = AppContext.BaseDirectory;
            string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(executable))) + @"\Db\AbpDapperTest.mdf";
            _connectionString = $@"Data Source=(localdb)\MsSqlLocalDb;Integrated Security=True;AttachDbFilename={path}";

            Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString = _connectionString;

            AbpSession.UserId = 1;
            AbpSession.TenantId = 1;
        }
    }
}