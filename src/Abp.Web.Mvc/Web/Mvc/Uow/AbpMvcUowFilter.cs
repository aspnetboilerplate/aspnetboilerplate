using System.Web;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Web.Mvc.Configuration;
using Abp.Web.Mvc.Extensions;

namespace Abp.Web.Mvc.Uow
{
    public class AbpMvcUowFilter: IActionFilter, ITransientDependency
    {
        public const string UowHttpContextKey = "__AbpUnitOfWork";

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpMvcConfiguration _configuration;

        public AbpMvcUowFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IAbpMvcConfiguration configuration)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            var methodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (methodInfo == null)
            {
                return;
            }

            var unitOfWorkAttr =
                UnitOfWorkAttribute.GetUnitOfWorkAttributeOrNull(methodInfo) ??
                _configuration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                return;
            }

            SetCurrentUow(
                filterContext.HttpContext,
                _unitOfWorkManager.Begin(unitOfWorkAttr.CreateOptions())
            );
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.IsChildAction)
            {
                return;
            }

            var uow = GetCurrentUow(filterContext.HttpContext);
            if (uow == null)
            {
                return;
            }

            try
            {
                if (filterContext.Exception == null)
                {
                    uow.Complete();
                }
            }
            finally
            {
                uow.Dispose();
                SetCurrentUow(filterContext.HttpContext, null);
            }
        }

        private static IUnitOfWorkCompleteHandle GetCurrentUow(HttpContextBase httpContext)
        {
            return httpContext.Items[UowHttpContextKey] as IUnitOfWorkCompleteHandle;
        }

        private static void SetCurrentUow(HttpContextBase httpContext, IUnitOfWorkCompleteHandle uow)
        {
            httpContext.Items[UowHttpContextKey] = uow;
        }
    }
}
