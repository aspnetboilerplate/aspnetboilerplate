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
        public void Insert_by_dapper_should_change_creationaudit()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                _productDapperRepository.Insert(new Product("TShirt"));

                Product product = _productDapperRepository.GetList(x => x.Name == "TShirt").FirstOrDefault();

                product.ShouldNotBeNull();
                product.TenantId.ShouldBe(AbpSession.TenantId);
                product.CreationTime.ShouldNotBeNull();
                product.CreatorUserId.ShouldBeNull();

                uow.Complete();
            }
        }

        [Fact]
        public void Update_by_dapper_repository_should_update_tenantId_and_lastmodified_audits()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                _productDapperRepository.Insert(new Product("TShirt"));

                Product product = _productDapperRepository.GetList(x => x.Name == "TShirt").FirstOrDefault();

                product.Name = "Pants";

                _productDapperRepository.Update(product);

                product.ShouldNotBeNull();
                product.TenantId.ShouldBe(AbpSession.TenantId);
                product.CreationTime.ShouldNotBeNull();
                product.LastModifierUserId.ShouldBe(AbpSession.UserId);

                uow.Complete();
            }
        }
    }
}
