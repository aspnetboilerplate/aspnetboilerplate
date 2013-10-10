using Abp.Domain.Services;
using Taskever.Domain.Business.Acitivities;

namespace Taskever.Domain.Services
{
    public interface IActivityService : IDomainService
    {
        void AddActivity(ActivityInfo eventHistoryData);
    }
}