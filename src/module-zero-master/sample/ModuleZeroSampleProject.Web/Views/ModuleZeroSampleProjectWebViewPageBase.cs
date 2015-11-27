using Abp.Web.Mvc.Views;

namespace ModuleZeroSampleProject.Web.Views
{
    public abstract class ModuleZeroSampleProjectWebViewPageBase : ModuleZeroSampleProjectWebViewPageBase<dynamic>
    {

    }

    public abstract class ModuleZeroSampleProjectWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected ModuleZeroSampleProjectWebViewPageBase()
        {
            LocalizationSourceName = ModuleZeroSampleProjectConsts.LocalizationSourceName;
        }
    }
}