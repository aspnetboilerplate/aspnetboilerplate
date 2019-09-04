using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Abp.Authorization
{
    public interface IAuthorizationHelper
    {
        Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes);
        
        void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes);

        Task AuthorizeAsync(MethodInfo methodInfo, Type type);
        
        void Authorize(MethodInfo methodInfo, Type type);
    }
}