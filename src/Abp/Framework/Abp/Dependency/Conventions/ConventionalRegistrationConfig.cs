namespace Abp.Dependency.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConventionalRegistrationConfig
    {
        /// <summary>
        /// Install all Installers or not? 
        /// </summary>
        public bool InstallInstallers { get; set; } // TODO: Document better

        /// <summary>
        /// Creates a new <see cref="ConventionalRegistrationConfig"/> object.
        /// </summary>
        public ConventionalRegistrationConfig()
        {
            InstallInstallers = true;
        }
    }
}