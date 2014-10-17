using Abp.Configuration.Startup;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.EntityFramework.Tests.Repositories
{
    public class DbContext_Create_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Should_Set_ConnectionString_If_Configured()
        {
            LocalIocManager.IocContainer.Register(
                Classes.FromThisAssembly().BasedOn<AbpDbContext>()
                    .WithServiceSelf().LifestyleTransient()
                    .Configure(c => c.DynamicParameters(
                        (kernel, dynamicParams) =>
                        {
                            if (!kernel.HasComponent(typeof (IAbpStartupConfiguration)))
                            {
                                return;
                            }

                            var defaultConnectionString = kernel.Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString;
                            if (string.IsNullOrWhiteSpace(defaultConnectionString))
                            {
                                return;
                            }

                            dynamicParams["nameOrConnectionString"] = defaultConnectionString;
                        })));

            var context1 = LocalIocManager.Resolve<MyDbContext>();
            context1.CalledConstructorWithConnectionString.ShouldBe(false);

            LocalIocManager.Register<IAbpStartupConfiguration, AbpStartupConfiguration>();

            var context2 = LocalIocManager.Resolve<MyDbContext>();
            context2.CalledConstructorWithConnectionString.ShouldBe(false);

            LocalIocManager.Resolve<IAbpStartupConfiguration>().DefaultNameOrConnectionString = "Server=localhost;Database:test;User=sa;Password=123";

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
        }
    }
}