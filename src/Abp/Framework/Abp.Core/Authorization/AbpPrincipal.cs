using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Abp.Authorization
{
    public class AbpPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; private set; }

        public AbpPrincipal(AbpIdentity identity)
        {
            Identity = identity;
        }
    }
}
