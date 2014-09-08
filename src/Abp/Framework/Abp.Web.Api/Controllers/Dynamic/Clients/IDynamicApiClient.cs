namespace Abp.WebApi.Controllers.Dynamic.Clients
{
    /// <summary>
    /// Defines interface of a client to use a remote Web Api service.
    /// </summary>
    /// <typeparam name="TService">Service type</typeparam>
    public interface IDynamicApiClient<out TService>
    {
        /// <summary>
        /// Url of the service.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// The service object.
        /// </summary>
        TService Service { get; }
    }
}
