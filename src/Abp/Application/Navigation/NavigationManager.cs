using System.Collections.Generic;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    internal class NavigationManager : INavigationManager, ISingletonDependency
    {
        private readonly INavigationConfiguration _configuration;

        private readonly IIocResolver _iocResolver;

        public NavigationManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;

            Menus = new Dictionary<string, MenuDefinition>
            {
                {"MainMenu", new MenuDefinition("MainMenu", new FixedLocalizableString("Main menu"))}
                //TODO: Localization for "Main menu"
            };
        }

        public IDictionary<string, MenuDefinition> Menus { get; }

        public MenuDefinition MainMenu
        {
            get { return Menus["MainMenu"]; }
        }

        public void Initialize()
        {
            var context = new NavigationProviderContext(this);

            foreach (var providerType in _configuration.Providers)
            {
                var provider = (NavigationProvider) _iocResolver.Resolve(providerType);
                provider.SetNavigation(context);
            }
        }
    }
}