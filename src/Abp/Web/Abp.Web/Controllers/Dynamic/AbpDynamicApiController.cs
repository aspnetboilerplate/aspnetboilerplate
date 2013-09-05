using Abp.Services;

namespace Abp.Web.Controllers.Dynamic
{
    /// <summary>
    /// This class is used as base class for dynamically created ApiControllers.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public class AbpDynamicApiController<T> : AbpApiController
    {

    }
}