using Abp.Domain.Services;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities.Activities;

namespace Taskever.Domain.Services
{
    public interface IActivityService : IDomainService
    {
        void AddActivity(Activity activity);
    }
}