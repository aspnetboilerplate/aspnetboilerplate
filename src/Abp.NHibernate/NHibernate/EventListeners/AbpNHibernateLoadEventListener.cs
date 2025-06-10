using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.NHibernate.Uow;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.NHibernate.EventListeners
{
    internal class AbpNHibernateLoadEventListener : DefaultLoadEventListener
    {
        protected override object DoLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
        {
            var result = base.DoLoad(@event, persister, keyToLoad, options);
            if (@event.Session.IsFilterEnabled(AbpDataFilters.SoftDelete))
            {
                if (result is ISoftDelete softDeletable)
                {
                    if (softDeletable.IsDeleted) return null;
                }
            }

            return result;
        }

        protected override async Task<object> DoLoadAsync(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options,
            CancellationToken cancellationToken)
        {
            var result = await base.DoLoadAsync(@event, persister, keyToLoad, options, cancellationToken);
            if (@event.Session.IsFilterEnabled(AbpDataFilters.SoftDelete))
            {
                if (result is ISoftDelete softDeletable)
                {
                    if (softDeletable.IsDeleted) return null;
                }
            }

            return result;
        }
    }
}