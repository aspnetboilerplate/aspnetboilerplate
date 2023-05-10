using System.Reflection;
using Abp.Dependency;
using Abp.HtmlSanitizer.Configuration;
using Abp.Modules;
using Ganss.Xss;

namespace Abp.HtmlSanitizer
{
    [DependsOn(typeof(AbpKernelModule))]    
    public class AbpHtmlSanitizerModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IHtmlSanitizerConfiguration, HtmlSanitizerConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            IocManager.Register<IHtmlSanitizer, Ganss.Xss.HtmlSanitizer>(DependencyLifeStyle.Transient);
        }
    }
}