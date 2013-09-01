using System.Web.Http;
using Abp.Web.Controllers;
using AttributeRouting.Web.Http.WebHost;
using Taskever.Web.App_Start;
using System.Reflection;

[assembly: WebActivator.PreApplicationStartMethod(typeof(AttributeRoutingHttpConfig), "Start")]

namespace Taskever.Web.App_Start 
{
    public static class AttributeRoutingHttpConfig
	{
		public static void RegisterRoutes(HttpRouteCollection routes) 
		{    
			// See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			// To debug routes locally using the built in ASP.NET development server, go to /routes.axd

            routes.MapHttpAttributeRoutes();
            routes.MapHttpAttributeRoutes(c => c.AddRoutesFromAssembly(Assembly.GetAssembly(typeof(AbpApiController))));
		}

        public static void Start() 
		{
            RegisterRoutes(GlobalConfiguration.Configuration.Routes);
        }
    }
}
