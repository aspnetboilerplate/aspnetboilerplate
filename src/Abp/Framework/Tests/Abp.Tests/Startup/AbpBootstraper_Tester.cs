using System;
using System.Collections.Generic;
using Abp.Modules;
using Abp.Startup;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Abp.Tests.Startup
{
    public class AbpBootstraper_Tester : TestBaseWithSelfIocManager
    {
        private readonly AbpBootstrapper _bootstrapper;

        public AbpBootstraper_Tester()
        {
            _bootstrapper = new AbpBootstrapper {IocManager = LocalIocManager};
        }

        [Fact]
        public void Should_Initialize_Bootstrapper()
        {
            _bootstrapper.Initialize();
        }

        [Fact]
        public void Should_Call_Module_Events_Once()
        {
            LocalIocManager.IocContainer.Register(Component.For<IModuleFinder>().ImplementedBy<MyTestModuleFinder>());
            
            MyTestModule.ClearCounters();

            _bootstrapper.Initialize();
            _bootstrapper.Dispose();

            MyTestModule.PreInitializeCount.ShouldBe(1);
            MyTestModule.InitializeCount.ShouldBe(1);
            MyTestModule.PostInitializeCount.ShouldBe(1);
            MyTestModule.ShutdownCount.ShouldBe(1);
        }

        public override void Dispose()
        {
            _bootstrapper.Dispose();
            base.Dispose();
        }
    }

    public class MyTestModuleFinder : IModuleFinder
    {
        public List<Type> FindAll()
        {
            return new List<Type>
                   {
                       typeof (MyTestModule)
                   };
        }
    }

    public class MyTestModule : AbpModule
    {
        public static int PreInitializeCount { get; private set; }

        public static int InitializeCount { get; private set; }

        public static int PostInitializeCount { get; private set; }

        public static int ShutdownCount { get; private set; }

        public static void ClearCounters()
        {
            PreInitializeCount = 0;
            InitializeCount = 0;
            PostInitializeCount = 0;
            ShutdownCount = 0;
        }

        public override void PreInitialize(IAbpInitializationContext context)
        {
            IocManager.ShouldNotBe(null);

            base.PreInitialize(context);
            PreInitializeCount++;
        }

        public override void Initialize(IAbpInitializationContext context)
        {
            base.Initialize(context);
            InitializeCount++;
        }

        public override void PostInitialize(IAbpInitializationContext context)
        {
            base.PostInitialize(context);
            PostInitializeCount++;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            ShutdownCount++;
        }
    }
}
