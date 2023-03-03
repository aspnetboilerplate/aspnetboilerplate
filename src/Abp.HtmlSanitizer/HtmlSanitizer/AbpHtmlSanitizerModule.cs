using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Ganss.Xss;

namespace Abp.HtmlSanitizer.HtmlSanitizer;

[DependsOn(typeof(AbpKernelModule))]    
public class AbpHtmlSanitizerModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        
        IocManager.Register<IHtmlSanitizer, Ganss.Xss.HtmlSanitizer>(DependencyLifeStyle.Transient);
    }
}