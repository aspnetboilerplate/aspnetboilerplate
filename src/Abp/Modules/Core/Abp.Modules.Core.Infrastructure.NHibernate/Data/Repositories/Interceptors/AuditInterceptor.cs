using System;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;
using Abp.Utils.Extensions;
using Abp.Utils.Extensions.Collections;
using Castle.DynamicProxy;

namespace Abp.Modules.Core.Data.Repositories.Interceptors
{
    //TODO: Implement this in NHibernate events!
    public class AuditInterceptor : IInterceptor
    {
        private readonly IAbpUserRepository _userRepository;

        public AuditInterceptor(IAbpUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.MethodInvocationTarget.Name == "Insert")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is ICreationAudited)
                    {
                        invocation.Arguments[0].As<ICreationAudited>().CreationTime = DateTime.Now;
                        if (invocation.Arguments[0].As<ICreationAudited>().CreatorUserId == null)
                        {
                            invocation.Arguments[0].As<ICreationAudited>().CreatorUserId = AbpUser.CurrentUserId;
                        }
                    }
                }

                invocation.Proceed();
            }
            else if (invocation.MethodInvocationTarget.Name == "Update")
            {
                if (!invocation.Arguments.IsNullOrEmpty())
                {
                    if (invocation.Arguments[0] is IModificationAudited)
                    {
                        invocation.Arguments[0].As<IModificationAudited>().LastModificationTime = DateTime.Now;
                        if (invocation.Arguments[0].As<IModificationAudited>().LastModifierUserId == null)
                        {
                            invocation.Arguments[0].As<IModificationAudited>().LastModifierUserId = AbpUser.CurrentUserId;
                        }
                    }
                }

                invocation.Proceed();
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}