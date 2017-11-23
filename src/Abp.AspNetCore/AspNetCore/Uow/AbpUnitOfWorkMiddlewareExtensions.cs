using System;
using Abp.AspNetCore.Uow;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class AbpUnitOfWorkMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app, Action<UnitOfWorkMiddlewareOptions> optionsAction = null)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<UnitOfWorkMiddlewareOptions>>().Value;
            optionsAction?.Invoke(options);
            return app.UseMiddleware<AbpUnitOfWorkMiddleware>();
        }
    }
}
