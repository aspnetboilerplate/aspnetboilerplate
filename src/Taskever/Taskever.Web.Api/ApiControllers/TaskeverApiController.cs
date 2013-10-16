using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.WebApi.Controllers;

namespace Taskever.Web.ApiControllers
{
    public class TaskeverApiController : AbpApiController
    {
        public TaskeverApiController()
        {
            LocalizationSourceName = "Taskever"; //TODO: Make constant!
        }
    }
}
