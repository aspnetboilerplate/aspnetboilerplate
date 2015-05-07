using System.Linq;
using System.Reflection;
using Abp.Runtime.Session;

namespace Abp.Auditing
{
    public static class AuditingHelper
    {
        public static bool ShouldSaveAudit(MethodInfo methodInfo, IAuditingConfiguration configuration, IAbpSession abpSession, bool defaultValue = false)
        {
            if (configuration == null || !configuration.IsEnabled)
            {
                return false;
            }

            if (!configuration.IsEnabledForAnonymousUsers && (abpSession == null || !abpSession.UserId.HasValue))
            {
                return false;
            }

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(AuditedAttribute)))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute)))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.IsDefined(typeof(AuditedAttribute)))
                {
                    return true;
                }

                if (classType.IsDefined(typeof(DisableAuditingAttribute)))
                {
                    return false;
                }

                if (configuration.Selectors.Any(selector => selector.Predicate(classType)))
                {
                    return true;
                }
            }

            return defaultValue;
        }
    }
}