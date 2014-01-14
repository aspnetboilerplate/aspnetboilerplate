using Abp.Domain.Services;

namespace Taskever.Activities
{
    public interface IActivityService : IDomainService
    {
        void AddActivity(Activity activity);
    }
}