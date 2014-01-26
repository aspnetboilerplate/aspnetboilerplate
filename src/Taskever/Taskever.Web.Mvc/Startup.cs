using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Taskever.Web.Mvc.Startup))]
namespace Taskever.Web.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
