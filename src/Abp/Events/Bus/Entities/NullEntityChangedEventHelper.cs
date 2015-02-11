namespace Abp.Events.Bus.Entities
{
    /// <summary>
    /// Null-object implementation of <see cref="IEntityChangedEventHelper"/>.
    /// </summary>
    public class NullEntityChangedEventHelper : IEntityChangedEventHelper
    {
        /// <summary>
        /// Gets single instance of <see cref="NullEventBus"/> class.
        /// </summary>
        public static NullEntityChangedEventHelper Instance { get { return SingletonInstance; } }
        private static readonly NullEntityChangedEventHelper SingletonInstance = new NullEntityChangedEventHelper();

        private NullEntityChangedEventHelper()
        {

        }

        public void TriggerEntityCreatedEvent(object entity)
        {
            
        }

        public void TriggerEntityUpdatedEvent(object entity)
        {
            
        }

        public void TriggerEntityDeletedEvent(object entity)
        {
            
        }
    }
}