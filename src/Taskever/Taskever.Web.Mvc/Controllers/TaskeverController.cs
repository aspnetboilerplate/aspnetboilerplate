using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Mvc.Controllers
{
    public class TaskeverController : AbpController
    {
        public TaskeverController()
        {
            LocalizationSourceName = "Taskever";
        }
    }
}