namespace Abp.Web.Startup
{
    internal class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }
    }
}