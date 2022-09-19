using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.NHibernate.EventListeners
{
    internal class AbpNHibernateLoadEventListener : DefaultLoadEventListener
    {
        private readonly IIocManager _iocManager;
        private readonly Lazy<IUnitOfWork> _unitOfWork;

        public AbpNHibernateLoadEventListener(IIocManager iocManager)
        {
            _iocManager = iocManager;

            _unitOfWork =
                new Lazy<IUnitOfWork>(() => _iocManager.Resolve<IUnitOfWork>());
        }
        protected override object DoLoad(LoadEvent @event, IEntityPersister persister, EntityKey keyToLoad, LoadType options)
        {
            var result = base.DoLoad(@event, persister, keyToLoad, options);
            if (_unitOfWork.Value.IsFilterEnabled(AbpDataFilters.SoftDelete))
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
            if (_unitOfWork.Value.IsFilterEnabled(AbpDataFilters.SoftDelete))
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