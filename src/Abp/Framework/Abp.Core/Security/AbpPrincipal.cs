using System.Security.Principal;

namespace Abp.Security
{
    //TODO: Inherit from GenericPrincipal and move this class out of Core!
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
