using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.Zero.SampleApp.Editions
{
    public class EditionManager : AbpEditionManager
    {
        public EditionManager(
            IRepository<Edition> editionRepository,
            IAbpZeroFeatureValueStore featureValueStore,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
               editionRepository,
               featureValueStore,
               unitOfWorkManager)
        {
        }
    }
}
