using System;
using System.Data.Common;
using System.Linq;
using Abp.Collections;
using Abp.Modules;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using ModuleZeroSampleProject.EntityFramework;
using ModuleZeroSampleProject.Migrations.Data;

namespace ModuleZeroSampleProject.Tests
{
    public abstract class SampleProjectTestBase : AbpIntegratedTestBase
    {
        protected SampleProjectTestBase()
        {
            //Fake DbConnection using Effort!
            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
                );

            //Creating initial data
            UsingDbContext(context => new InitialDataBuilder().Build(context));
            
            AbpSession.TenantId = 1;
        }

        protected override void AddModules(ITypeList<AbpModule> modules)
        {
            base.AddModules(modules);
            modules.Add<ModuleZeroSampleProjectApplicationModule>();
            modules.Add<ModuleZeroSampleProjectDataModule>();
        }

        public void UsingDbContext(Action<ModuleZeroSampleProjectDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<ModuleZeroSampleProjectDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        public T UsingDbContext<T>(Func<ModuleZeroSampleProjectDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<ModuleZeroSampleProjectDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}
