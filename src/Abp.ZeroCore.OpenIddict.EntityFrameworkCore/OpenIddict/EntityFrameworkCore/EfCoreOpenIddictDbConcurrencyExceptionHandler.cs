using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace Abp.OpenIddict.EntityFrameworkCore;

public class EfCoreOpenIddictDbConcurrencyExceptionHandler : IOpenIddictDbConcurrencyExceptionHandler, ITransientDependency
{
    public virtual Task HandleAsync(AbpDbConcurrencyException exception)
    {
        if (exception != null &&
            exception.InnerException is DbUpdateConcurrencyException updateConcurrencyException)
        {
            foreach (var entry in updateConcurrencyException.Entries)
            {
                // Reset the state of the entity to prevents future calls to SaveChangesAsync() from failing.
                entry.State = EntityState.Unchanged;
            }
        }

        return Task.CompletedTask;
    }
}