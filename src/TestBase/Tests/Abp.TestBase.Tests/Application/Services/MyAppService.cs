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
        public string RequiredStringValue { get; set; }
    }

    public class MyMethodOutput : IOutputDto
    {
        public int Result { get; set; }
    }
}
