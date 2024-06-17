using System.Threading;

namespace Abp.EntityFrameworkCore;

public class AbpEfCoreCurrentDbContext
{
    public readonly AsyncLocal<AbpDbContext> Current = new AsyncLocal<AbpDbContext>();
}
