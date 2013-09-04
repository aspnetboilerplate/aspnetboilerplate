using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Services;
using Abp.Web.Controllers;
using Castle.DynamicProxy;

namespace Taskever.Web.Api
{
    public class AbpServiceApiController : AbpApiController
    {

    }

    public class AbpServiceApiControllerInterceptor<TService> : IInterceptor
    {


        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }

    public class Selector : DefaultHttpControllerSelector
    {
        public Selector(HttpConfiguration configuration)
            : base(configuration)
        {

        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controller = base.SelectController(request);
            return new HttpControllerDescriptor(null, "", null);
        }
    }

}