using System.Threading.Tasks;
using Abp.AspNetCore.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Uow
{
    public class AbpUowPageFilter : IAsyncPageFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IAbpAspNetCoreConfiguration _aspnetCoreConfiguration;
        private readonly IUnitOfWorkDefaultOptions _unitOfWorkDefaultOptions;

        public AbpUowPageFilter(
            IUnitOfWorkManager unitOfWorkManager,
            IAbpAspNetCoreConfiguration aspnetCoreConfiguration,
            IUnitOfWorkDefaultOptions unitOfWorkDefaultOptions)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _aspnetCoreConfiguration = aspnetCoreConfiguration;
            _unitOfWorkDefaultOptions = unitOfWorkDefaultOptions;
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var unitOfWorkAttr = _unitOfWorkDefaultOptions
                                     .GetUnitOfWorkAttributeOrNull(context.HandlerMethod.MethodInfo) ??
                                 _aspnetCoreConfiguration.DefaultUnitOfWorkAttribute;

            if (unitOfWorkAttr.IsDisabled)
            {
                await next();
                return;
            }

            var uowOpts = new UnitOfWorkOptions
            {
                IsTransactional = unitOfWorkAttr.IsTransactional,
                IsolationLevel = unitOfWorkAttr.IsolationLevel,
                Timeout = unitOfWorkAttr.Timeout,
                Scope = unitOfWorkAttr.Scope
            };

            using (var uow = _unitOfWorkManager.Begin(uowOpts))
            {
                var result = await next();
                if (result.Exception == null || result.ExceptionHandled)
                {
                    await uow.CompleteAsync();
                }
            }
        }
    }
}