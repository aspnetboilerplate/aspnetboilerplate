using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Category
    {
        [Key]
        public virtual int Id { get; set; }

        [Audited]
        public virtual string DisplayName { get; set; }
    }
}
