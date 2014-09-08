namespace Abp.WebApi.Controllers.Dynamic.Clients
{
    /// <summary>
    /// TODO: This class and namespace is being developed. See https://github.com/aspnetboilerplate/aspnetboilerplate/issues/66 
    /// </summary>
    public static class DynamicApiClientBuilder
    {
        public static IApiClientBuilder<TService> For<TService>(string url)
        {
            return new ApiClientBuilder<TService>(url);
        }
    }
}