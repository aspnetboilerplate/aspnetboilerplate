using System.Reflection;
using Abp.Dependency;
using Abp.HtmlSanitizer.Configuration;
using Abp.Modules;
using Ganss.Xss;

namespace Abp.HtmlSanitizer;

[DependsOn(typeof(AbpKernelModule))]
public class AbpHtmlSanitizerModule : AbpModule
{
    public override void PreInitialize()
    {
        IocManager.Register<IHtmlSanitizerConfiguration, HtmlSanitizerConfiguration>();
        IocManager.Register<IHtmlSanitizer, Ganss.Xss.HtmlSanitizer>(DependencyLifeStyle.Transient);
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}