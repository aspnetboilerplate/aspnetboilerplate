namespace Abp.Events.Bus.Entities
{
    /// <summary>
    ///     Null-object implementation of <see cref="IEntityChangeEventHelper" />.
    /// </summary>
    public class NullEntityChangeEventHelper : IEntityChangeEventHelper
    {
        private NullEntityChangeEventHelper()
        {
        }

        /// <summary>
        ///     Gets single instance of <see cref="NullEventBus" /> class.
        /// </summary>
        public static NullEntityChangeEventHelper Instance { get; } = new NullEntityChangeEventHelper();

        public void TriggerEntityCreatingEvent(object entity)
        {
        }

        public void TriggerEntityCreatedEventOnUowCompleted(object entity)
        {
        }

        public void TriggerEntityUpdatingEvent(object entity)
        {
        }

        public void TriggerEntityUpdatedEventOnUowCompleted(object entity)
        {
        }

        public void TriggerEntityDeletingEvent(object entity)
        {
        }

        public void TriggerEntityDeletedEventOnUowCompleted(object entity)
        {
        }
    }
}