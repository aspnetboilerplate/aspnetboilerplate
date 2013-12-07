using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Taskever.Application.Services.Dto.TaskeverUsers;

namespace Taskever.Application.Services
{
    public interface ITaskeverUserService : IApplicationService
    {
        GetUserProfileOutput GetUserProfile(GetUserProfileInput input);
    }
}
