using Abp.Web.Mvc.Controllers;

namespace Reation_APP.CMS.Web.Controllers
{
    /// <summary>
    /// Derive all Controllers from this class.
    /// </summary>
    public abstract class CMSControllerBase : AbpController
    {
        protected CMSControllerBase()
        {
            LocalizationSourceName = CMSConsts.LocalizationSourceName;
        }
    }
}