using Abp.AspNetCore.Mvc.Views;

namespace AbpAspNetCoreDemo.Views
{
    public abstract class MyDemoRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected MyDemoRazorPage()
        {
            LocalizationSourceName = "AbpAspNetCoreDemoModule";
        }
    }
}
