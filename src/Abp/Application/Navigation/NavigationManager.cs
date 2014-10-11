using System.Collections.Generic;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    internal class NavigationManager : INavigationManager, ISingletonDependency
    {
        public IDictionary<string, Menu> Menus { get; private set; }

        public Menu MainMenu
        {
            get { return Menus["MainMenu"]; }
        }

        private readonly IIocResolver _iocResolver;
        private readonly INavigationConfiguration _configuration;

        public NavigationManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;

            Menus = new Dictionary<string, Menu>
                    {
                        {"MainMenu", new Menu("MainMenu", new FixedLocalizableString("Main menu"))} //TODO: Localization for "Main menu"
                    };
        }

        public void Initialize()
        {
            var context = new NavigationProviderContext(this);

            foreach (var providerType in _configuration.Providers)
            {
                var provider = (INavigationProvider)_iocResolver.Resolve(providerType);
                provider.SetNavigation(context);
            }
        }
    }
}