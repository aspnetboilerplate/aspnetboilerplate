using Abp.Authorization;
using Abp.Modules.Core.Entities;
using Abp.Utils.Extensions;

namespace Abp.Modules.Core.Authorization
{
    public class AbpAuthorizeAttribute : AbpCoreAuthorizeAttribute
    {
        public AbpAuthorizeAttribute()
        {
            Features = new string[0];
        }

        public override bool Authorize()
        {
            if (!base.Authorize())
            {
                return false;
            }

            if (User.Current == null)
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
