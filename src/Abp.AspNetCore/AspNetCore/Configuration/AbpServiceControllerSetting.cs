using System.Reflection;

namespace Abp.AspNetCore.Configuration
{
    public class AbpServiceControllerSetting
    {
        /// <summary>
        /// "app".
        /// </summary>
        public const string DefaultServiceModuleName = "app";

        public string ModuleName { get; }

        public Assembly Assembly { get; }

        public bool UseConventionalHttpVerbs { get; }

        public AbpServiceControllerSetting(string moduleName, Assembly assembly, bool useConventionalHttpVerbs)
        {
            ModuleName = moduleName;
            Assembly = assembly;
            UseConventionalHttpVerbs = useConventionalHttpVerbs;
        }
    }
}