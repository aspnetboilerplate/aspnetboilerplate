using Abp.Authorization;
using Abp.Utils.Extensions;
using System.Threading;
using Abp.Utils.Extensions.Collections;

namespace Abp.Modules.Core.Authorization
{
    public class AbpAuthorizeAttribute : AbpCoreAuthorizeAttribute
    {
        public override bool Authorize()
        {
            if (!base.Authorize())
            {
                return false;
            }

            if (Features.IsNullOrEmpty())
            {
                return true;
            }
            
            //TODO: Check if this user has access to one of these features!

            return false;
        }
    }
}
