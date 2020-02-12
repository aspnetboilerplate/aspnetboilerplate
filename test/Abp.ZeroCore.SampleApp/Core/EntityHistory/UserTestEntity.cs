using System;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class UserTestEntity : AggregateRoot, IHasCreationTime
    {
        public DateTime CreationTime { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int Age { get; set; }
    }
}
