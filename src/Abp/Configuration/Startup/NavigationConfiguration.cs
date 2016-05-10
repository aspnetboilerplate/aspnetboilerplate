using Abp.Application.Navigation;
using Abp.Collections;

namespace Abp.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public NavigationConfiguration()
        {
            Providers = new TypeList<NavigationProvider>();
        }

        public ITypeList<NavigationProvider> Providers { get; }
    }
}