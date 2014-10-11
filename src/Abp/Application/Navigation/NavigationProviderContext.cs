namespace Abp.Application.Navigation
{
    internal class NavigationProviderContext : INavigationProviderContext
    {
        public INavigationManager Manager { get; private set; }

        public NavigationProviderContext(NavigationManager manager)
        {
            Manager = manager;
        }
    }
}