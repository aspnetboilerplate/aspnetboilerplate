using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain;

// IsDeleted is stored with a converter where false is not 0, so the soft delete filter only
// matches rows when its constant goes through the column's converter.
public class IntSoftDeleteNote : Entity, ISoftDelete
{
    public const int NotDeletedValue = 5;
    public const int DeletedValue = 9;

    public string Title { get; set; }

    public bool IsDeleted { get; set; }
}
