using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace Abp.NHibernate.EventListeners
{
    internal class AbpNHibernateLoadEventListener : DefaultLoadEventListener
    {
        protected override object DoLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)//1-4
        {
            var result = base.DoLoad(@event, persister, keyToLoad, options);

            if (result is ISoftDelete softDeletable && softDeletable.IsDeleted) return null;

            return result;
        }

        protected override async Task<object> DoLoadAsync(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options,
            CancellationToken cancellationToken)
        {
            var result = await base.DoLoadAsync(@event, persister, keyToLoad, options, cancellationToken);

            if (result is ISoftDelete softDeletable && softDeletable.IsDeleted) return null;

            return result;
        }
    }
}