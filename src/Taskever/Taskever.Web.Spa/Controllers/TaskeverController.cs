using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Abp.Web.Mvc.Controllers;

namespace Taskever.Web.Controllers
{
    public class TaskeverController : AbpController
    {
        public TaskeverController()
        {
            LocalizationSourceName = "Taskever";
        }
    }
}