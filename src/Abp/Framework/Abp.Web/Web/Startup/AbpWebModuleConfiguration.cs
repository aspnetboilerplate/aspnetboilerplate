using Abp.Startup.Configuration;

namespace Abp.Web.Startup
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        /// <summary>
        /// Singletion instance.
        /// </summary>
        internal static AbpWebModuleConfiguration Instance { get { return _instance; } }
        private static readonly AbpWebModuleConfiguration _instance = new AbpWebModuleConfiguration();

        private AbpWebModuleConfiguration()
        {

        }
    }
}