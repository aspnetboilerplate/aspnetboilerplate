using Abp.Application;
using Abp.Authorization;
using Abp.Modules.Core.Entities;
using Abp.Utils.Extensions;

namespace Abp.Modules.Core.Authorization
{
    public class AbpCoreAuthorizeAttribute : AbpAuthorizeAttribute
    {
        public AbpCoreAuthorizeAttribute()
        {
            Features = new string[0];
        }

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

            if (User.Current == null)
            {
                return true;
            }

            //TODO: Check if this user has access to one of these features!

            return false;
        }
    }
}
