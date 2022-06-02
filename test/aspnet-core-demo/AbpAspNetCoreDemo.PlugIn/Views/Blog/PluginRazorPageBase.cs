using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace AbpAspNetCoreDemo.PlugIn.Views;

public abstract class PluginRazorPageBase<TModel> : AbpRazorPage<TModel>
{
    [RazorInject] public IAbpSession AbpSession { get; set; }
}
