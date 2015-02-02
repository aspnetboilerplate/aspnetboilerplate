using System.ComponentModel.DataAnnotations;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using Abp.Runtime.Validation;
using Shouldly;
using Xunit;

namespace Abp.TestBase.Tests.Application.Services
{
    public class Validation_Tests : AbpIntegratedTestBase
    {
        private readonly IMyAppService _myAppService;

        public Validation_Tests()
        {
            LocalIocManager.Register<IMyAppService, MyAppService>(DependencyLifeStyle.Transient);
            _myAppService = LocalIocManager.Resolve<IMyAppService>();
        }

        [Fact]
        public void Should_Work_Proper_With_Right_Inputs()
        {
            var output = _myAppService.MyMethod(new MyMethodInput { MyStringValue = "test" });
            output.Result.ShouldBe(42);
        }

        [Fact]
        public void Should_Not_Work_Proper_With_Wrong_Inputs()
        {
            Assert.Throws<AbpValidationException>(() => _myAppService.MyMethod(new MyMethodInput())); //MyStringValue is not supplied!
            Assert.Throws<AbpValidationException>(() => _myAppService.MyMethod(new MyMethodInput { MyStringValue = "a" })); //MyStringValue's min length should be 3!
        }

        #region Nested Classes

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

        public class MyMethodInput : IInputDto
        {
            [Required]
            [MinLength(3)]
            public string MyStringValue { get; set; }
        }

        public class MyMethodOutput : IOutputDto
        {
            public int Result { get; set; }
        }

        #endregion
    }
}
