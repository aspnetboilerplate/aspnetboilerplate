using Abp.Web.Api.ProxyScripting.Configuration;

namespace Abp.Web.Configuration
{
    /// <summary>
    /// Used to configure ABP Web module.
    /// </summary>
    public interface IAbpWebModuleConfiguration
    {
        /// <summary>
        /// If this is set to true, all exception and details are sent directly to clients on an error.
        /// Default: false (ABP hides exception details from clients except special exceptions.)
        /// </summary>
        bool SendAllExceptionsToClients { get; set; }

        /// <summary>
        /// Used to configure Api proxy scripting.
        /// </summary>
        IApiProxyScriptingConfiguration ApiProxyScripting { get; }
    }
}