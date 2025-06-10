using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Abp.EntityFrameworkCore;

#pragma warning disable EF1001
public class AbpEntityQueryProvider : EntityQueryProvider
{
    protected AbpEfCoreCurrentDbContext AbpEfCoreCurrentDbContext { get; }
    protected ICurrentDbContext CurrentDbContext { get; }

    public AbpEntityQueryProvider(
        IQueryCompiler queryCompiler,
        AbpEfCoreCurrentDbContext abpEfCoreCurrentDbContext,
        ICurrentDbContext currentDbContext)
        : base(queryCompiler)
    {
        AbpEfCoreCurrentDbContext = abpEfCoreCurrentDbContext;
        CurrentDbContext = currentDbContext;
    }

    public override object Execute(Expression expression)
    {
        using (AbpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as AbpDbContext))
        {
            return base.Execute(expression);
        }
    }

    public override TResult Execute<TResult>(Expression expression)
    {
        using (AbpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as AbpDbContext))
        {
            return base.Execute<TResult>(expression);
        }
    }

    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
    {
        using (AbpEfCoreCurrentDbContext.Use(CurrentDbContext.Context as AbpDbContext))
        {
            return base.ExecuteAsync<TResult>(expression, cancellationToken);
        }
    }
}
#pragma warning restore EF1001