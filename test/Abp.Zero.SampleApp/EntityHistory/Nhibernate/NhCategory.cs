using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhCategory
    {
        [Key]
        public virtual int Id { get; set; }

        [Audited]
        public virtual string DisplayName { get; set; }
    }
}
