using Abp.Dependency;
using Abp.Localization.Sources;

namespace Abp.WebApi.Controllers.Dynamic.Scripting.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public static class LocalizationScriptManager
    {
        private static readonly ILocalizationSourceManager LocalizationSourceManager;

        private static string _script;

        private static readonly object SyncObj = new object();

        static LocalizationScriptManager()
        {
            LocalizationSourceManager = IocHelper.Resolve<ILocalizationSourceManager>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetScript()
        {
            //TODO: Generate according to current language!!! So, it's not true to store single _script cache! Use dictionary or MemoryCache instead?
            //TODO: Also drop cache if localization file is changed!

            lock (SyncObj)
            {
                if (_script == null)
                {
                    _script = new LocalizationScriptBuilder(LocalizationSourceManager.GetAllSources()).BuildAll();
                }

                return _script;
            }
        }
    }
}
