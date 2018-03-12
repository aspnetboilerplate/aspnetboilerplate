using Abp.Dependency;
using Abp.PlugIns;

namespace Abp
{
    public class AbpBootstrapperOptions
    {
        /// <summary>
        /// Used to disable all interceptors added by ABP.
        /// </summary>
        public bool DisableAllInterceptors { get; set; }

        /// <summary>
        /// IIocManager that is used to bootstrap the ABP system. If set to null, uses global <see cref="Abp.Dependency.IocManager.Instance"/>
        /// </summary>
        public IIocManager IocManager { get; set; }

        /// <summary>
        /// List of plugin sources.
        /// </summary>
        public PlugInSourceList PlugInSources { get; }

        public AbpBootstrapperOptions()
        {
            IocManager = Abp.Dependency.IocManager.Instance;
            PlugInSources = new PlugInSourceList();
        }
    }
}
