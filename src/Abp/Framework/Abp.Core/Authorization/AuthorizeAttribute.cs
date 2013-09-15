using Abp.Application;
using System;
using Abp.Utils.Extensions;

namespace Abp.Authorization
{
    public class AbpAuthorizeAttribute : Attribute
    {
        public string[] Features { get; set; }

        public AbpAuthorizeAttribute()
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
