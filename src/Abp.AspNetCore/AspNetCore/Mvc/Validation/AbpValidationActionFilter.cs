using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Aspects;
using Abp.Dependency;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Validation
{
    public class AbpValidationActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;

        public AbpValidationActionFilter(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Validation))
            {
                using (var validator = _iocResolver.ResolveAsDisposable<MvcActionInvocationValidator>())
                {
                    validator.Object.Initialize(context);
                    validator.Object.Validate();
                }

                await next();
            }
        }
    }
}
