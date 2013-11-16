using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Entities;
using Taskever.Domain.Enums;

namespace Taskever.Application.Services.Dto.Activities
{
    public class ActivityDto : EntityDto<long>, IOutputDto
    {
        public virtual ActivityAction Action { get; set; }

        public virtual string ActionName { get; set; }

        public virtual ActivityInfo ActivityInfo { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public static ActivityDto CreateFromActivity(Activity activity)
        {
            return new ActivityDto
                       {
                           Id = activity.Id,
                           Action = activity.Action,
                           ActionName = activity.Action.ToString(),
                           ActivityInfo = activity.Action.CreateActivityInfo(activity.Data),
                           CreationTime = activity.CreationTime
                       };
        }
    }
}
