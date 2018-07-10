using System;
using Abp.Web;

namespace AbpAspNetMvcDemo
{
    public class MvcApplication : AbpWebApplication<AbpAspNetMvcDemoModule>
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);
        }

        protected override void Application_BeginRequest(object sender, EventArgs e)
        {
            base.Application_BeginRequest(sender, e);
        }
    }
}
