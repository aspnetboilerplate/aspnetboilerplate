namespace Abp.Application.Navigation
{
    /// <summary>
    /// Provides infrastructure to set navigation.
    /// </summary>
    public interface INavigationProviderContext
    {
        /// <summary>
        /// Gets main menu of the application to modify.
        /// </summary>
        Menu MainMenu { get; }
    }
}