using Abp.Dapper.Repositories;
using Abp.Dapper.Tests.Entities;
using Abp.Domain.Repositories;

using Shouldly;

using Xunit;

namespace Abp.Dapper.Tests
{
    public class DapperRepository_Tests : DapperApplicationTestBase
    {
        private readonly IDapperRepository<Product> _productDapperRepository;
        private readonly IRepository<Product> _productRepository;

        public DapperRepository_Tests()
        {
            _productDapperRepository = Resolve<IDapperRepository<Product>>();
            _productRepository = Resolve<IRepository<Product>>();
        }

        //[Fact(Skip = "Effort does not support Dapper queries.")]
        public void DapperRepositories_Should_Work_Within_A_UnitOfWorkScope()
        {
            _productDapperRepository.Insert(new Product("TShirt", "Red", 5) { Id = 1 });

            Product productFromEf = _productRepository.Get(1);

            _productDapperRepository.GetList(x => x.Name == "TShirt").ShouldNotBeNull();

            productFromEf.Name = "Pants";
            _productDapperRepository.Update(productFromEf);

            _productDapperRepository.GetListPaged(x => x.Name == "Pants", 1, 10, true, product => product.Name, product => product.Color);
        }
    }
}
