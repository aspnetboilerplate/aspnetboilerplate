namespace Adorable.Web.Configuration
{
    internal class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }
    }
}