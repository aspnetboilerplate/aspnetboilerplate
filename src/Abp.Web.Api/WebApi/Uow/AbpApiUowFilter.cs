using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.WebApi.Validation;

namespace Abp.WebApi.Uow
{
    public class AbpApiUowFilter : IActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public bool AllowMultiple => false;

        public AbpApiUowFilter(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var methodInfo = actionContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return await continuation();
            }

            var unitOfWorkAttr = UnitOfWorkAttribute.GetUnitOfWorkAttributeOrNull(methodInfo) ??
                                 new UnitOfWorkAttribute();

            if (unitOfWorkAttr.IsDisabled)
            {
                return await continuation();
            }

            using (var uow = _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions()))
            {
                var result = await continuation();
                await uow.CompleteAsync();
                return result;
            }
        }
    }
}