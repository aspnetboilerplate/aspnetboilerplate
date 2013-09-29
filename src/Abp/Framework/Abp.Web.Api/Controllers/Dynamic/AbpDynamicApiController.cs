namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// This class is used as base class for all dynamically created ApiControllers.
    /// </summary>
    /// <typeparam name="T">Type of the proxied object</typeparam>
    public class AbpDynamicApiController<T> : AbpApiController
    {

    }
}