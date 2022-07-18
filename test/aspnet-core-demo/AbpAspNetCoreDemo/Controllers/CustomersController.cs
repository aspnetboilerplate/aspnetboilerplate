using Abp.AspNetCore.OData.Controllers;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AbpAspNetCoreDemo.Core.Domain;

namespace AbpAspNetCoreDemo.Controllers
{
    public class CustomersController : AbpODataEntityController<Customer>, ITransientDependency
    {
        public CustomersController(IRepository<Customer> repository) : base(repository)
        {
            
        }
    }
}