using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using MySpaProject.Tasks.Dtos;

namespace MySpaProject.Tasks
{
    public interface ITaskAppService : IApplicationService
    {
        GetAllTasksOutput GetAllTasks();
    }
}
