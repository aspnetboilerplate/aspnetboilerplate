using System.Web;
using Abp.Dependency;
using Taskever.Startup.Dependency.Installers;
using Taskever.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(PreAppStart), "Run")]

namespace Taskever.Web.Mvc
{
    public class PreAppStart
    {
        public static void Run()
        {
            IocManager.Instance.IocContainer.Install(new Log4NetInstaller());
        }
    }

    //public class Test
    //{
    //    public static void Start()
    //    {
    //        //Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(MyModule));
    //    }
    //}

    //public class MyModule : IHttpModule
    //{
    //    public void Init(HttpApplication context)
    //    {
            
    //        context.MapRequestHandler += (sender, e) =>
    //                                         {
    //                                             var app = ((HttpApplication)sender);
    //                                             var ext = VirtualPathUtility.GetExtension(app.Request.FilePath);
    //                                         };
    //        context.BeginRequest += (sender, e) =>
    //        {
    //            var app = ((HttpApplication)sender);
    //            var ext = VirtualPathUtility.GetExtension(app.Request.FilePath);
    //            //var response = app.Response;
    //            //response.Write("MyModule.BeginRequest");
    //        };
    //    }

    //    public void Dispose()
    //    {
    //        //throw new NotImplementedException();
    //    }
    //}
}