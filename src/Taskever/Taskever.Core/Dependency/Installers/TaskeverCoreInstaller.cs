using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Taskever.Localization.Resources;

namespace Taskever.Dependency.Installers
{
    public class TaskeverCoreInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITaskeverLocalizationSource>().ImplementedBy<TaskeverLocalizationSource>()
                );
        }
    }
}
