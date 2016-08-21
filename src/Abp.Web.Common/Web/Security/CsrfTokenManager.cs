using System.Reflection;
using Abp.Dependency;
using Abp.Reflection;

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

        public bool ShouldValidate(MethodInfo methodInfo, HttpVerb httpVerb, bool defaultValue = false)
        {
            if (!Configuration.IsEnabled)
            {
                return false;
            }

            if (!methodInfo.IsDefined(typeof(ValidateCsrfTokenAttribute), true))
            {
                if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableCsrfTokenValidationAttribute>(methodInfo) != null)
                {
                    return false;
                }

                if (Configuration.IgnoredHttpVerbs.Contains(httpVerb))
                {
                    return false;
                }
            }

            return defaultValue;
        }
    }
}