using System.Linq;

using Abp.Dapper.Repositories;
using Abp.Dapper.Tests.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

using Shouldly;

using Xunit;

namespace Abp.Dapper.Tests
{
    public class DapperRepository_Tests : DapperApplicationTestBase
    {
        private readonly IDapperRepository<Product> _productDapperRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DapperRepository_Tests()
        {
            _productDapperRepository = Resolve<IDapperRepository<Product>>();
            _productRepository = Resolve<IRepository<Product>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
        }

        [Fact]
        public void Insert_Should_Work()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                _productDapperRepository.Insert(new Product("TShirt"));

                Product product = _productDapperRepository.GetList(x => x.Name == "TShirt").FirstOrDefault();

                product.ShouldNotBeNull();

                uow.Complete();
            }
        }
    }
}
