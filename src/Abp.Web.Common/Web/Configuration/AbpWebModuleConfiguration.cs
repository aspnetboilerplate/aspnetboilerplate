namespace Abp.Web.Configuration
{
    internal class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }
    }
}