namespace Abp.Application.Navigation
{
    internal class NavigationProviderContext : INavigationProviderContext
    {
        public NavigationProviderContext(INavigationManager manager)
        {
            Manager = manager;
        }

        public INavigationManager Manager { get; }
    }
}