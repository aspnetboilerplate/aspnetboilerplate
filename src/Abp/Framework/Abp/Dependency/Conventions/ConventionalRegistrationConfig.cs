namespace Abp.Dependency.Conventions
{
    /// <summary>
    /// 
    /// </summary>
    public class ConventionalRegistrationConfig
    {
        public bool InstallInstallers { get; set; }

        public ConventionalRegistrationConfig()
        {
            InstallInstallers = true;
        }
    }
}