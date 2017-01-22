using Abp.Application.Services;
using Abp.Dependency;
using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests.Application.Services
{
    /// <summary>
    /// Should support working without database or a unit of work.
    /// </summary>
    public class ApplicationWithoutDb_Tests : AbpIntegratedTestBase<AbpKernelModule>
    {
        private readonly IMyAppService _myAppService;
        public ApplicationWithoutDb_Tests()
        {
            LocalIocManager.Register<IMyAppService, MyAppService>(DependencyLifeStyle.Transient);
            _myAppService = Resolve<IMyAppService>();
        }

        [Fact]
        public void Test1()
        {
            var output = _myAppService.MyMethod(new MyMethodInput {MyStringValue = "test"});
            output.Result.ShouldBe(42);
        }

        #region Sample Application service

        public interface IMyAppService
        {
            MyMethodOutput MyMethod(MyMethodInput input);
        }

        public class MyAppService : IMyAppService, IApplicationService
        {
            public MyMethodOutput MyMethod(MyMethodInput input)
            {
                return new MyMethodOutput { Result = 42 };
            }
        }

        public class MyMethodInput 
        {
            public string MyStringValue { get; set; }
        }

        public class MyMethodOutput
        {
            public int Result { get; set; }
        }

        #endregion
    }
}
