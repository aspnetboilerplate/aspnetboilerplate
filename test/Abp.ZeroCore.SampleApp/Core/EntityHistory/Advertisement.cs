using Abp.Auditing;
using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class Advertisement : FullAuditedEntity
    {
        public Advertisement()
        {
        }

        public Advertisement(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; set; }

        [DisableAuditing]
        public string Content { get; set; }
    }
}

