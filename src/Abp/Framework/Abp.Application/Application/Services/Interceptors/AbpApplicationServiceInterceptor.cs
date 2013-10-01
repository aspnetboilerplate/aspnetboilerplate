using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Abp.Application.Services.Dto;
using Abp.Validation;
using Castle.DynamicProxy;

namespace Abp.Application.Services.Interceptors
{
    public class AbpApplicationServiceInterceptor : IInterceptor
    {
        private readonly IMethodInvocationValidator _invocationValidator;

        public AbpApplicationServiceInterceptor(IMethodInvocationValidator invocationValidator)
        {
            _invocationValidator = invocationValidator;
        }

        public void Intercept(IInvocation invocation)
        {
            _invocationValidator.Validate(invocation.InvocationTarget, invocation.Method, invocation.Arguments);
            invocation.Proceed();
        }
    }
}
