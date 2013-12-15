using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;

namespace Taskever.Application.Services
{
    public interface INotificationService : IApplicationService
    {
        void Notify(INotification notification);
    }
}
