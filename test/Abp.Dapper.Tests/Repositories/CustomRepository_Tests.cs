using Abp.Domain.Uow;
using Shouldly;
using Xunit;

namespace Abp.Dapper.Tests.Repositories
{
    public class CustomRepository_Tests : DapperApplicationTestBase
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly CustomRepository _customRepository;

        public CustomRepository_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _customRepository = Resolve<CustomRepository>();
        }

        [Fact]
        public void Repository_Test()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                _customRepository.AddProducts();
                var products = _customRepository.GetProducts();

                products.Count.ShouldBe(3);
                products.ShouldContain(x => x.Name == "watch");
                products.ShouldContain(x => x.Name == "phone");
                products.ShouldContain(x => x.Name == "pad");

                uow.Complete();
            }
        }

        [Fact]
        public void Repository_Transaction_Test()
        {
            using (_unitOfWorkManager.Begin())
            {
                _customRepository.AddProducts();
                //uow.Complete();
            }

            using (_unitOfWorkManager.Begin())
            {
                var products = _customRepository.GetProducts();
                products.Count.ShouldBe(0);
            }
        }

    }
}