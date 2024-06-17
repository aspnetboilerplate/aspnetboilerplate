using System.Threading;

namespace Abp.EntityFrameworkCore;

public class AbpEfCoreCurrentDbContext
{
    public AsyncLocal<AbpDbContext> Current { get; } = new AsyncLocal<AbpDbContext>();
}
