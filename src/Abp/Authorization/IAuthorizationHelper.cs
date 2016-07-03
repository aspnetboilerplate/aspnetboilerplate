using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Authorization
{
    public interface IAuthorizationHelper
    {
        Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes);

        Task AuthorizeAsync(MethodInfo methodInfo);
    }
}