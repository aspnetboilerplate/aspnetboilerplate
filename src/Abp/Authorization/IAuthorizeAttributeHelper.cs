using System.Collections.Generic;
using System.Threading.Tasks;

namespace Adorable.Authorization
{
    internal interface IAuthorizeAttributeHelper
    {
        Task AuthorizeAsync(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes);
        
        Task AuthorizeAsync(IAbpAuthorizeAttribute authorizeAttribute);
        
        void Authorize(IEnumerable<IAbpAuthorizeAttribute> authorizeAttributes);
        
        void Authorize(IAbpAuthorizeAttribute authorizeAttribute);
    }
}