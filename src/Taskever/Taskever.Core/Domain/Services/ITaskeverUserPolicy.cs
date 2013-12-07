using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Modules.Core.Domain.Entities;
using Taskever.Domain.Policies;

namespace Taskever.Domain.Services
{
    public interface ITaskeverUserPolicy : IPolicy
    {
        bool CanSeeProfile(User requesterUser, User targetUser);
    }
}
