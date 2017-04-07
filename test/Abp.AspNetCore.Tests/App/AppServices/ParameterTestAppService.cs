using Abp.Application.Services;
using Abp.AspNetCore.App.Models;

namespace Abp.AspNetCore.App.AppServices
{
    public class ParameterTestAppService : ApplicationService
    {
        public string GetComplexInput(SimpleViewModel model, bool testBool)
        {
            return "42";
        }
    }
}