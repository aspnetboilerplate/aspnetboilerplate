using System.Security.Principal;

namespace Abp.Authorization
{
    public class AbpPrincipal : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return true;
        }

        public IIdentity Identity { get; private set; }

        public AbpPrincipal()
        {
            Identity = new AbpIdentity();
        }

        public AbpPrincipal(AbpIdentity identity)
        {
            Identity = identity;
        }
    }
}
