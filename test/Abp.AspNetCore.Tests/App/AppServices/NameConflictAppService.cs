using Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Abp.AspNetCore.App.AppServices
{
    public class NameConflictAppService : ApplicationService
    {
        [HttpGet]
        public string GetConstantString()
        {
            return "return-value-from-app-service";
        }
    }
}