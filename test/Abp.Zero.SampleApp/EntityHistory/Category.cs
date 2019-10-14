using Abp.Auditing;
using System.ComponentModel.DataAnnotations;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Audited]
        public string DisplayName { get; set; }
    }
}
