using Abp.Application;
using System;
using Abp.Utils.Extensions;

namespace Abp.Authorization
{
    public class AbpCoreAuthorizeAttribute : Attribute
    {
        public string[] Features { get; set; }

        public AbpCoreAuthorizeAttribute()
        {
            Features = new string[0];
        }

        public virtual bool Authorize()
        {
            if (Features.IsNullOrEmpty())
            {
                return true;
            }

            foreach (var feature in Features)
            {
                if (Edition.Current.Features.ContainsKey(feature))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
