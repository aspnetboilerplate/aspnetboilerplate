using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dapper.Filters.Query;
using Abp.Dapper.Repositories;
using Abp.Dapper.Tests.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using DapperExtensions;
using Shouldly;
using Xunit;

namespace Abp.Dapper.Tests
{
    public class DapperRepository_Tests : DapperApplicationTestBase
    {
        private readonly IDapperRepository<Product> _productDapperRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<ProductDetail> _productDetailRepository;
        private readonly IDapperRepository<ProductDetail> _productDetailDapperRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IDapperRepository<Person> _personDapperRepository;
        private readonly IDapperRepository<Good> _goodDapperRepository;
        private readonly IDapperQueryFilterExecuter _dapperQueryFilterExecuter;

        public DapperRepository_Tests()
        {
            _productDapperRepository = Resolve<IDapperRepository<Product>>();
            _productRepository = Resolve<IRepository<Product>>();
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();
            _productDetailRepository = Resolve<IRepository<ProductDetail>>();
            _productDetailDapperRepository = Resolve<IDapperRepository<ProductDetail>>();
            _personRepository = Resolve<IRepository<Person>>();
            _personDapperRepository = Resolve<IDapperRepository<Person>>();
            _goodDapperRepository = Resolve<IDapperRepository<Good>>();
            _dapperQueryFilterExecuter = Resolve<IDapperQueryFilterExecuter>();
        }

        [Fact]
        public async Task Dapper_Repository_Tests()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                //---Insert operation should work and tenant, creation audit properties must be set---------------------
                _productDapperRepository.Insert(new Product("TShirt"));
                Product insertedProduct = _productDapperRepository.GetAll(x => x.Name == "TShirt").FirstOrDefault();

                insertedProduct.ShouldNotBeNull();
                insertedProduct.TenantId.ShouldBe(AbpSession.TenantId);
                insertedProduct.CreationTime.ShouldNotBeNull();
                insertedProduct.CreatorUserId.ShouldBe(AbpSession.UserId);

                //----Update operation should work and Modification Audits should be set---------------------------
                _productDapperRepository.Insert(new Product("TShirt"));
                Product productToUpdate = _productDapperRepository.GetAll(x => x.Name == "TShirt").FirstOrDefault();
                productToUpdate.Name = "Pants";
                _productDapperRepository.Update(productToUpdate);

                productToUpdate.ShouldNotBeNull();
                productToUpdate.TenantId.ShouldBe(AbpSession.TenantId);
                productToUpdate.CreationTime.ShouldNotBeNull();
                productToUpdate.LastModifierUserId.ShouldBe(AbpSession.UserId);

                //---Get method should return single-------------------------------------------------------------------
                _productDapperRepository.Insert(new Product("TShirt"));
                Action getAction = () => _productDapperRepository.Single(x => x.Name == "TShirt");

                getAction.ShouldThrow<InvalidOperationException>("Sequence contains more than one element");

                //----Select * from syntax should work---------------------------------
                IEnumerable<Product> products = _productDapperRepository.Query("select * from Products");

                products.Count().ShouldBeGreaterThan(0);

                //------------Ef and Dapper should work under same transaction---------------------
                Product productFromEf = _productRepository.FirstOrDefault(x => x.Name == "TShirt");
                Product productFromDapper = _productDapperRepository.Single(productFromEf.Id);

                productFromDapper.Name.ShouldBe(productFromEf.Name);
                productFromDapper.TenantId.ShouldBe(productFromEf.TenantId);

                //------Soft Delete should work for Dapper--------------
                _productDapperRepository.Insert(new Product("SoftDeletableProduct"));

                Product toSoftDeleteProduct = _productDapperRepository.Single(x => x.Name == "SoftDeletableProduct");

                _productDapperRepository.Delete(toSoftDeleteProduct);

                toSoftDeleteProduct.IsDeleted.ShouldBe(true);
                toSoftDeleteProduct.DeleterUserId.ShouldBe(AbpSession.UserId);
                toSoftDeleteProduct.TenantId.ShouldBe(AbpSession.TenantId);

                Product softDeletedProduct = _productRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                softDeletedProduct.ShouldBeNull();

                Product softDeletedProductFromDapper = _productDapperRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                softDeletedProductFromDapper.ShouldBeNull();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    Product softDeletedProductWhenFilterDisabled = _productRepository.FirstOrDefault(x => x.Name == "SoftDeletableProduct");
                    softDeletedProductWhenFilterDisabled.ShouldNotBeNull();

                    Product softDeletedProductFromDapperWhenFilterDisabled = _productDapperRepository.Single(x => x.Name == "SoftDeletableProduct");
                    softDeletedProductFromDapperWhenFilterDisabled.ShouldNotBeNull();
                }

                using (AbpSession.Use(2, 266))
                {
                    int productWithTenant2Id = _productDapperRepository.InsertAndGetId(new Product("ProductWithTenant2"));

                    var productWithTenant2 = _productRepository.Get(productWithTenant2Id);

                    productWithTenant2.TenantId.ShouldBe(1); //Not sure about that?,Because we changed TenantId to 2 in this scope !!! Abp.TenantId = 2 now NOT 1 !!!
                }

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    int productWithTenant3Id = _productDapperRepository.InsertAndGetId(new Product("ProductWithTenant3"));

                    Product productWithTenant3 = _productRepository.Get(productWithTenant3Id);

                    productWithTenant3.TenantId.ShouldBe(3);
                }

                Product productWithTenantId3FromDapper = _productDapperRepository.FirstOrDefault(x => x.Name == "ProductWithTenant3");
                productWithTenantId3FromDapper.ShouldBeNull();

                Product p = await _productDapperRepository.FirstOrDefaultAsync(x => x.Status == Status.Active);
                p.ShouldNotBeNull();

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    Product productWithTenantId3FromDapperInsideTenantScope = _productDapperRepository.FirstOrDefault(x => x.Name == "ProductWithTenant3");
                    productWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }

                //About issue-#2091
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int productWithTenantId40 = _productDapperRepository.InsertAndGetId(new Product("ProductWithTenantId40"));

                    Product productWithTenant40 = _productRepository.Get(productWithTenantId40);

                    productWithTenant40.TenantId.ShouldBe(AbpSession.TenantId);
                    productWithTenant40.CreatorUserId.ShouldBe(AbpSession.UserId);
                }

                //Second DbContext tests
                int productDetailId = _productDetailRepository.InsertAndGetId(new ProductDetail("Woman"));
                _productDetailDapperRepository.Get(productDetailId).ShouldNotBeNull();

                uow.Complete();
            }
        }

        //About issue-#3990
        [Fact]
        public void Should_Insert_Only_Have_IMustHaveTenant()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int personWithTenantId40 = _personDapperRepository.InsertAndGetId(new Person("PersonWithTenantId40"));

                    Person personWithTenant40 = _personRepository.Get(personWithTenantId40);

                    personWithTenant40.TenantId.ShouldBe(AbpSession.TenantId.Value);
                }
            }

        }

        [Fact]
        public async Task Dapper_Repository_Count_Should_Return_Correct_Value_For_Nullable_Int_Filter()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    await _goodDapperRepository.InsertAsync(new Good { Name = "AbpTest" });
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    int? id = 1;

                    var dapperCount = await _goodDapperRepository.CountAsync(a => a.Id != id && a.Name == "AbpTest");
                    dapperCount.ShouldBe(0);
                }

                uow.Complete();
            }

        }
    }
}
