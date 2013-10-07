using System;
using Abp.Domain.Entities;
using Abp.Modules.Core.Domain.Entities;
using Abp.Modules.Core.Domain.Entities.Utils;

namespace Taskever.Domain.Entities.EventHistories
{
    public class EventHistory : Entity<long>, ICreationAudited
    {
        public virtual EventHistoryFormatter Formatter { get; set; }

        public virtual EventHistoryType HistoryType { get; set; }

        public virtual short HistoryVersion { get; set; }

        public virtual string HistoryText { get; set; }

        public virtual DateTime CreationTime { get; set; }

        public virtual User CreatorUser { get; set; }
    }
}
