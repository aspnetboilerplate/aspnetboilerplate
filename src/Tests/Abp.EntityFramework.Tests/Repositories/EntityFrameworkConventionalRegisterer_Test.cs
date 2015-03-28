using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.EntityFramework.Dependency;
using Abp.Tests;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class EntityFrameworkConventionalRegisterer_Test : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Set_ConnectionString_If_Configured()
        {
            new EntityFrameworkConventionalRegisterer()
                .RegisterAssembly(
                    new ConventionalRegistrationContext(
                        Assembly.GetExecutingAssembly(),
                        LocalIocManager,
                        new ConventionalRegistrationConfig()
                        ));

            //Should call default constructor since IAbpStartupConfiguration is not configured. 
            var context1 = LocalIocManager.Resolve<MyDbContext>();
            context1.CalledConstructorWithConnectionString.ShouldBe(false);

            LocalIocManager.Register<IAbpStartupConfiguration, AbpStartupConfiguration>();

            //Should call default constructor since IAbpStartupConfiguration registered by IAbpStartupConfiguration.DefaultNameOrConnectionString is not set. 
            var context2 = LocalIocManager.Resolve<MyDbContext>();
            context2.CalledConstructorWithConnectionString.ShouldBe(false);

            LocalIocManager.Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString = "Server=localhost;Database=test;User=sa;Password=123";

            //Should call constructor with nameOrConnectionString since IAbpStartupConfiguration.DefaultNameOrConnectionString is set.
            var context3 = LocalIocManager.Resolve<MyDbContext>();
            context3.CalledConstructorWithConnectionString.ShouldBe(true);
        }

        public class MyDbContext : AbpDbContext
        {
            public bool CalledConstructorWithConnectionString { get; private set; }

            public MyDbContext()
            {

            }

            public MyDbContext(string nameOrConnectionString)
                : base(nameOrConnectionString)
            {
                CalledConstructorWithConnectionString = true;
            }

            public override void Initialize()
            {

            }
        }
    }
}