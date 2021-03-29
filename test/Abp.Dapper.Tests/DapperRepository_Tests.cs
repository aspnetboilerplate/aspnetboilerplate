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
                await _productDapperRepository.InsertAsync(new Product("TShirt"));
                Product insertedProduct = await _productDapperRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");

                insertedProduct.ShouldNotBeNull();
                insertedProduct.TenantId.ShouldBe(AbpSession.TenantId);
                ((DateTime?) insertedProduct.CreationTime).ShouldNotBe(null);
                insertedProduct.CreatorUserId.ShouldBe(AbpSession.UserId);

                //----Update operation should work and Modification Audits should be set---------------------------
                await _productDapperRepository.InsertAsync(new Product("TShirt"));
                Product productToUpdate = await _productDapperRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");
                productToUpdate.Name = "Pants";
                await _productDapperRepository.UpdateAsync(productToUpdate);

                productToUpdate.ShouldNotBeNull();
                productToUpdate.TenantId.ShouldBe(AbpSession.TenantId);
                ((DateTime?) productToUpdate.CreationTime).ShouldNotBe(null);
                productToUpdate.LastModifierUserId.ShouldBe(AbpSession.UserId);

                //---Get method should return single-------------------------------------------------------------------
                await _productDapperRepository.InsertAsync(new Product("TShirt"));
                Action getAction = () => _productDapperRepository.Single(x => x.Name == "TShirt");

                getAction.ShouldThrow<InvalidOperationException>("Sequence contains more than one element");

                //----Select * from syntax should work---------------------------------
                var queryResult = await _productDapperRepository.QueryAsync("select * from Products");

                IEnumerable<Product> products = queryResult;

                products.Count().ShouldBeGreaterThan(0);

                //------------Ef and Dapper should work under same transaction---------------------
                Product productFromEf = await _productRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");
                Product productFromDapper = await _productDapperRepository.SingleAsync(productFromEf.Id);

                productFromDapper.Name.ShouldBe(productFromEf.Name);
                productFromDapper.TenantId.ShouldBe(productFromEf.TenantId);

                //------Soft Delete should work for Dapper--------------
                await _productDapperRepository.InsertAsync(new Product("SoftDeletableProduct"));

                Product toSoftDeleteProduct = await _productDapperRepository
                    .SingleAsync(x => x.Name == "SoftDeletableProduct");

                await _productDapperRepository.DeleteAsync(toSoftDeleteProduct);

                toSoftDeleteProduct.IsDeleted.ShouldBe(true);
                toSoftDeleteProduct.DeleterUserId.ShouldBe(AbpSession.UserId);
                toSoftDeleteProduct.TenantId.ShouldBe(AbpSession.TenantId);

                Product softDeletedProduct = await _productRepository
                    .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                softDeletedProduct.ShouldBeNull();

                Product softDeletedProductFromDapper = await _productDapperRepository
                    .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                softDeletedProductFromDapper.ShouldBeNull();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    Product softDeletedProductWhenFilterDisabled = await _productRepository
                        .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                    softDeletedProductWhenFilterDisabled.ShouldNotBeNull();

                    Product softDeletedProductFromDapperWhenFilterDisabled = await _productDapperRepository
                        .SingleAsync(x => x.Name == "SoftDeletableProduct");

                    softDeletedProductFromDapperWhenFilterDisabled.ShouldNotBeNull();
                }

                using (AbpSession.Use(2, 266))
                {
                    int productWithTenant2Id = await _productDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenant2"));

                    var productWithTenant2 = await _productRepository.GetAsync(productWithTenant2Id);

                    productWithTenant2.TenantId
                        .ShouldBe(1); //Not sure about that?,Because we changed TenantId to 2 in this scope !!! Abp.TenantId = 2 now NOT 1 !!!
                }

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    int productWithTenant3Id = await _productDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenant3"));

                    Product productWithTenant3 = await _productRepository.GetAsync(productWithTenant3Id);

                    productWithTenant3.TenantId.ShouldBe(3);
                }

                Product productWithTenantId3FromDapper = await _productDapperRepository
                    .FirstOrDefaultAsync(x => x.Name == "ProductWithTenant3");

                productWithTenantId3FromDapper.ShouldBeNull();

                Product p = await _productDapperRepository.FirstOrDefaultAsync(x => x.Status == Status.Active);
                p.ShouldNotBeNull();

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    Product productWithTenantId3FromDapperInsideTenantScope = await _productDapperRepository
                        .FirstOrDefaultAsync(x => x.Name == "ProductWithTenant3");

                    productWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }

                //About issue-#2091
                using (_unitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int productWithTenantId40 = await _productDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenantId40"));

                    Product productWithTenant40 = await _productRepository.GetAsync(productWithTenantId40);

                    productWithTenant40.TenantId.ShouldBe(AbpSession.TenantId);
                    productWithTenant40.CreatorUserId.ShouldBe(AbpSession.UserId);
                }

                //Second DbContext tests
                var productDetailId = await _productDetailRepository
                    .InsertAndGetIdAsync(new ProductDetail("Woman"));

                (await _productDetailDapperRepository.GetAsync(productDetailId)).ShouldNotBeNull();

                await uow.CompleteAsync();
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
                    int personWithTenantId40 =
                        _personDapperRepository.InsertAndGetId(new Person("PersonWithTenantId40"));

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
                    await _goodDapperRepository.InsertAsync(new Good {Name = "AbpTest"});
                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    int? id = 1;

                    var dapperCount = await _goodDapperRepository.CountAsync(a => a.Id != id && a.Name == "AbpTest");
                    dapperCount.ShouldBe(0);
                }

                await uow.CompleteAsync();
            }
        }
    }
}
