using System.Reflection;
using System.Web.Mvc;
using Adorable.Modules;
using Adorable.Web.Mvc.Controllers;

namespace Adorable.Web.Mvc
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using Adorable.
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebMvcModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
        }
    }
}
