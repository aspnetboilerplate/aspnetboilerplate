using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.EFCore
{
    public class Category
    {
        [Key] public int Id { get; set; }

        [Audited] public string DisplayName { get; set; }
    }
}