using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace Abp.OpenIddict;

public interface IOpenIddictDbConcurrencyExceptionHandler
{
    Task HandleAsync(AbpDbConcurrencyException exception);
}