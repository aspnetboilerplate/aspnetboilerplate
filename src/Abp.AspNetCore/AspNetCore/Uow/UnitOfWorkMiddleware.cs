using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Abp.AspNetCore.Uow
{
    public class AbpUnitOfWorkMiddleware : IMiddleware, ITransientDependency
    {
        private readonly RequestDelegate _next;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UnitOfWorkMiddlewareOptions _options;

        public AbpUnitOfWorkMiddleware(
            RequestDelegate next,
            IUnitOfWorkManager unitOfWorkManager,
            IOptions<UnitOfWorkMiddlewareOptions> options)
        {
            _next = next;
            _unitOfWorkManager = unitOfWorkManager;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!_options.Filter(context))
            {
                await _next(context);
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(_options.OptionsFactory(context)))
            {
                await _next(context);
                await uow.CompleteAsync();
            }
        }
    }
}
