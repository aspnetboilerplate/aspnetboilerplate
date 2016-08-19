using Abp.Dependency;

namespace Abp.Web.Security
{
    public class CsrfTokenManager : ICsrfTokenManager, ITransientDependency
    {
        public ICsrfConfiguration Configuration { get; }

        public ICsrfTokenGenerator TokenGenerator { get; }

        public CsrfTokenManager(ICsrfConfiguration configuration, ICsrfTokenGenerator tokenGenerator)
        {
            Configuration = configuration;
            TokenGenerator = tokenGenerator;
        }
    }
}