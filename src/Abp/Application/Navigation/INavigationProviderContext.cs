namespace Abp.Application.Navigation
{
    /// <summary>
    /// Provides infrastructure to set navigation.
    /// </summary>
    public interface INavigationProviderContext
    {
        /// <summary>
        /// Gets a reference to the menu manager.
        /// </summary>
        INavigationManager Manager { get; }
    }
}