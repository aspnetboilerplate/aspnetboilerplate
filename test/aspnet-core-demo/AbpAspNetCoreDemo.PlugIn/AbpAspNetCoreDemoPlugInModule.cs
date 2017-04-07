using System.Reflection;
using Abp.AspNetCore;
using Abp.Modules;
using Abp.Resources.Embedded;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.Scs.Server;

namespace AbpAspNetCoreDemo.PlugIn
{
    [DependsOn(typeof(AbpAspNetCoreModule))]
    public class AbpAspNetCoreDemoPlugInModule : AbpModule
    {
        public override void PreInitialize()
        {
            //SCS package is just added to test dependency of a plugin module!
            var server = ScsServerFactory.CreateServer(new ScsTcpEndPoint(42000));

            Configuration.EmbeddedResources.Sources.Add(
                new EmbeddedResourceSet(
                    "/Views/",
                    Assembly.GetExecutingAssembly(),
                    "AbpAspNetCoreDemo.PlugIn.Views"
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
