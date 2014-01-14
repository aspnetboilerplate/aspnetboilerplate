using Abp.Events.Bus;
using Abp.Events.Bus.Factories;
using Abp.Events.Bus.Handlers;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Taskever.Startup.Dependency.Installers
{
    public class EventHandlersInstaller : IWindsorInstaller
    {
        private IEventBus _eventBus;

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            _eventBus = container.Resolve<IEventBus>();

            container.Kernel.ComponentRegistered += Kernel_ComponentRegistered;

            container.Register(Classes.FromThisAssembly().BasedOn<IEventHandler>().WithServiceSelf().LifestyleTransient());

            container.Kernel.ComponentRegistered -= Kernel_ComponentRegistered;
        }

        private void Kernel_ComponentRegistered(string key, IHandler handler)
        {
            /* This code checks if registering component implements any IEventHandler<TEventData> interface, if yes,
             * gets all event handler interfaces and registers type to Event Bus for each handling event.
             */
            if (!typeof(IEventHandler).IsAssignableFrom(handler.ComponentModel.Implementation))
            {
                return;
            }

            var interfaces = handler.ComponentModel.Implementation.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                {
                    continue;
                }

                var genericArgs = @interface.GetGenericArguments();
                if (genericArgs.Length == 1)
                {
                    _eventBus.Register(genericArgs[0], new IocHandlerFactory(handler.ComponentModel.Implementation));
                }
            }
        }
    }
}
