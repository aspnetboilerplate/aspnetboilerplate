using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Manages <see cref="Menu"/>s in the application.
    /// </summary>
    public interface IMenuManager
    {

    }

    public class MenuManager : IMenuManager, ISingletonDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly INavigationConfiguration _configuration;

        public MenuManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            _iocResolver = iocResolver;
            _configuration = configuration;
        }

        public void Initialize()
        {
            var context = new NavigationProviderContext();

            foreach (var providerType in _configuration.Providers)
            {
                var provider = _iocResolver.Resolve(providerType) as INavigationProvider;
                if (provider == null)
                {
                    throw new AbpInitializationException(
                        providerType.AssemblyQualifiedName +
                        " should implement " +
                        typeof (INavigationProvider).Name +
                        " in order to be a navigation provider."
                        );
                }

                provider.SetNavigation(context);
            }
        }
    }

    public class NavigationProviderContext : INavigationProviderContext
    {
        public Menu MainMenu { get; private set; }

        public NavigationProviderContext()
        {
            MainMenu = new Menu {Name = "MainMenu", DisplayName = new FixedLocalizableString("Main menu")};
        }
    }
}
