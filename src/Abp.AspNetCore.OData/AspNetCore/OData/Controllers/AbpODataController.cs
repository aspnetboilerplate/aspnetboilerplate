using Abp.Authorization;
using Abp.Domain.Uow;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Abp.AspNetCore.OData.Controllers;

public abstract class AbpODataController : ODataController
{
    public IUnitOfWorkManager UnitOfWorkManager { get; set; }

    public IPermissionChecker PermissionChecker { protected get; set; }

    protected AbpODataController()
    {
        PermissionChecker = NullPermissionChecker.Instance;
    }
}