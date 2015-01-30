using System.ComponentModel.DataAnnotations;
using Abp.Application.Services;
using Abp.Application.Services.Dto;

namespace Abp.TestBase.Tests.Application.Services
{
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
}
