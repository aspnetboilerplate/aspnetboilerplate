using Abp.Application.Navigation;
using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public ITypeList<INavigationProvider> Providers { get; private set; }

        public NavigationConfiguration()
        {
            Providers = new TypeList<INavigationProvider>();
        }
    }
}