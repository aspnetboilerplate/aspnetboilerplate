using System.Data.Common;

using Abp.TestBase;

using Castle.MicroKernel.Registration;

namespace Abp.Dapper.Tests
{
    public abstract class DapperApplicationTestBase : AbpIntegratedTestBase<AbpDapperTestModule>
    {
        protected override void PreInitialize()
        {
            base.PreInitialize();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );
        }
    }
}
