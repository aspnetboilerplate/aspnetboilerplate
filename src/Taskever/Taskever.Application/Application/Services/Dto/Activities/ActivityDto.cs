using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Taskever.Domain.Business.Acitivities;
using Taskever.Domain.Enums;

namespace Taskever.Application.Services.Dto.Activities
{
    public class ActivityDto : IOutputDto
    {
        public virtual ActivityAction Action { get; set; }

        public virtual ActivityInfo ActivityInfo { get; set; }

        public virtual DateTime CreationTime { get; set; }
    }
}
