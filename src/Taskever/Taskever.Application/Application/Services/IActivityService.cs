using Abp.Application.Services;
using Taskever.Domain.Business.Acitivities;

namespace Taskever.Application.Services
{
    public interface IActivityService : IApplicationService
    {
        void AddActivity(ActivityData eventHistoryData);
    }
}