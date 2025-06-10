using Abp.AspNetCore.OData.Controllers;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;

namespace AbpAspNetCoreDemo.Controllers;

public class ProductsDtoController : AbpODataDtoController<Product, ProductDto, ProductCreateInput>, ITransientDependency
{
    public ProductsDtoController(IRepository<Product> repository, IObjectMapper objectMapper) : base(repository, objectMapper)
    {
        GetPermissionName = "GetProductPermission";
        GetAllPermissionName = "GetAllProductsPermission";
        CreatePermissionName = "CreateProductPermission";
        UpdatePermissionName = "UpdateProductPermission";
        DeletePermissionName = "DeleteProductPermission";
    }
}