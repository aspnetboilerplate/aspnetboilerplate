using System.Linq;
using Abp.Authorization;
using Abp.Localization;
using Abp.Runtime.Session;
using Castle.DynamicProxy;

namespace Abp.MultiTenancy
{
    internal class MultiTenancySideInterceptor : IInterceptor
    {
        public IAbpSession AbpSession { get; set; }
        public ILocalizationManager LocalizationManager { get; set; }

        public MultiTenancySideInterceptor()
        {
            AbpSession = NullAbpSession.Instance;

            LocalizationManager = NullLocalizationManager.Instance;
        }

        public void Intercept(IInvocation invocation)
        {
            if (AbpSession.UserId.HasValue)
            {
                invocation.Proceed();
                return;
            }

            var methodInfo = invocation.MethodInvocationTarget;
            var multiTenancySideAttribute = methodInfo.GetCustomAttributes(true).OfType<MultiTenancySideAttribute>().FirstOrDefault()
                  ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<MultiTenancySideAttribute>().FirstOrDefault();

            if (multiTenancySideAttribute == null)
            {
                invocation.Proceed();
                return;
            }

            if (multiTenancySideAttribute.Side.HasFlag(MultiTenancySides.Host) && AbpSession.TenantId.HasValue)
            {
                throw new AbpAuthorizationException(
                    LocalizationManager.GetString(AbpConsts.LocalizationSourceName, "AnonymousTenantUserMustNotCallHostMethod")
                    );
            }
            else if (multiTenancySideAttribute.Side.HasFlag(MultiTenancySides.Tenant) && !AbpSession.TenantId.HasValue)
            {
                throw new AbpAuthorizationException(
                    LocalizationManager.GetString(AbpConsts.LocalizationSourceName, "AnonymousHostUserMustNotCallTenantMethod")
                    );
            }

            invocation.Proceed();
        }
    }
}