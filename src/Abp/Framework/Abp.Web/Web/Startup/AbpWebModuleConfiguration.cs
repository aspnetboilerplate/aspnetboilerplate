namespace Abp.Web.Startup
{
    public class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }
    }
}