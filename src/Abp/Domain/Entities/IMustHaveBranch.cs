using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.Domain.Entities
{
    public interface IMustHaveBranch
    {
        [Column("branch_id")]
        long BranchId { get; set; }
    }
}