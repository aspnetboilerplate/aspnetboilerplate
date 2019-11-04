using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Abp.Authorization;
using Abp.Domain.Uow;
using Microsoft.AspNet.OData;

namespace Abp.WebApi.OData.Controllers
{
    public abstract class AbpODataController : ODataController
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        protected IUnitOfWorkCompleteHandle UnitOfWorkCompleteHandler { get; private set; }

        protected bool IsDisposed { get; set; }

        public IPermissionChecker PermissionChecker { protected get; set; }

        protected AbpODataController()
        {
            PermissionChecker = NullPermissionChecker.Instance;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            UnitOfWorkCompleteHandler = UnitOfWorkManager.Begin();
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                UnitOfWorkCompleteHandler.Complete();
                UnitOfWorkCompleteHandler.Dispose();
            }

            IsDisposed = true;

            base.Dispose(disposing);
        }
    }
}