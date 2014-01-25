using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taskever.Startup.Dependency.Installers
{
    public class Log4NetInstaller : IWindsorInstaller
    {
        private static bool _isInstalled;

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (_isInstalled)
            {
                return;
            }

            container.AddFacility<LoggingFacility>(f => f.UseLog4Net().WithConfig("log4net.config"));

            _isInstalled = true;
        }
    }
}