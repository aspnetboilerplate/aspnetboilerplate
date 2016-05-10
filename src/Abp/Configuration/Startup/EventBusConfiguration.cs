namespace Abp.Configuration.Startup
{
    internal class EventBusConfiguration : IEventBusConfiguration
    {
        public EventBusConfiguration()
        {
            UseDefaultEventBus = true;
        }

        public bool UseDefaultEventBus { get; set; }
    }
}