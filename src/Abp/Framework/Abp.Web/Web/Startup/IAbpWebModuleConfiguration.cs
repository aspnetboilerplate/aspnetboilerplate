namespace Abp.Web.Startup
{
    public interface IAbpWebModuleConfiguration
    {
        bool SendAllExceptionsToClients { get; set; }
    }
}