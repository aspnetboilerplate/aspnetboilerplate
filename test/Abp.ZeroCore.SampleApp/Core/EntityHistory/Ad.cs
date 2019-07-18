using Abp.Auditing;
using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory
{
    [Audited]
    public class Ad : FullAuditedEntity
    {
        public Ad()
        {
        }

        public Ad(string name, string content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; set; }

        public string Content { get; set; }
    }
}

