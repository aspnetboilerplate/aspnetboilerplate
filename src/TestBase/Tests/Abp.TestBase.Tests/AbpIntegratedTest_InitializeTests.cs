using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests
{
    public class AbpIntegratedTest_InitializeTests : AbpIntegratedTest
    {
        private bool _preInitializeCalled;
        private bool _initializeCalled;
        private bool _postInitializeCalled;
        private bool _shutdownCalled;

        public AbpIntegratedTest_InitializeTests()
        {
            _preInitializeCalled.ShouldBe(true);
            _initializeCalled.ShouldBe(true);
            _postInitializeCalled.ShouldBe(true);
        }

        [Fact]
        public void SampleTestMethod()
        {
            _preInitializeCalled.ShouldBe(true);
            _initializeCalled.ShouldBe(true);
            _postInitializeCalled.ShouldBe(true);
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();
            _preInitializeCalled = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _initializeCalled = true;
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
            _postInitializeCalled = true;
        }

        protected override void Shutdown()
        {
            base.Shutdown();
            _shutdownCalled = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            _shutdownCalled.ShouldBe(true);
        }
    }
}
