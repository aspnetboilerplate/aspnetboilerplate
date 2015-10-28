using Abp.Web.Mvc.Views;

namespace MyAbpZeroProject.Web.Views
{
    public abstract class MyAbpZeroProjectWebViewPageBase : MyAbpZeroProjectWebViewPageBase<dynamic>
    {

    }

    public abstract class MyAbpZeroProjectWebViewPageBase<TModel> : AbpWebViewPage<TModel>
    {
        protected MyAbpZeroProjectWebViewPageBase()
        {
            LocalizationSourceName = MyAbpZeroProjectConsts.LocalizationSourceName;
        }
    }
}