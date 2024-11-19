using System;
using System.Threading;

namespace Abp.EntityFrameworkCore;

public class AbpEfCoreCurrentDbContext
{
    private readonly AsyncLocal<AbpDbContext> _current = new AsyncLocal<AbpDbContext>();

    public AbpDbContext Context => _current.Value;

    public IDisposable Use(AbpDbContext context)
    {
        var previousValue = Context;
        _current.Value = context;
        return new DisposeAction(() =>
        {
            _current.Value = previousValue;
        });
    }
}