using System.Threading.Tasks;

namespace Abp.AspNetCore.OpenIddict.Claims;

public interface IAbpOpenIddictClaimsPrincipalHandler
{
    Task HandleAsync(AbpOpenIddictClaimsPrincipalHandlerContext context);
}