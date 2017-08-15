using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Abp.IdentityFramework
{
    public class AbpIdentityResult : IdentityResult
    {
        public AbpIdentityResult()
        {
            
        }

        public AbpIdentityResult(IEnumerable<string> errors)
            : base(errors)
        {
            
        }

        public AbpIdentityResult(params string[] errors)
            :base(errors)
        {
            
        }

        public static AbpIdentityResult Failed(params string[] errors)
        {
            return new AbpIdentityResult(errors);
        }
    }
}