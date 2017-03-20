using System;
using System.Collections.Generic;
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
        public void Dapper_Repository_Tests()
        {
            using (IUnitOfWorkCompleteHandle uow = _unitOfWorkManager.Begin())
            {
                //---Insert operation should work and tenant, creation audit properties must be set---------------------
                _productDapperRepository.Insert(new Product("TShirt"));
                Product insertedProduct = _productDapperRepository.GetAll(x => x.Name == "TShirt").FirstOrDefault();

                insertedProduct.ShouldNotBeNull();
                insertedProduct.TenantId.ShouldBe(AbpSession.TenantId);
                insertedProduct.CreationTime.ShouldNotBeNull();
                insertedProduct.CreatorUserId.ShouldBeNull();

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

                    Product productWithTenant2 = _productRepository.Get(productWithTenant2Id);

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

                using (_unitOfWorkManager.Current.SetTenantId(3))
                {
                    Product productWithTenantId3FromDapperInsideTenantScope = _productDapperRepository.FirstOrDefault(x => x.Name == "ProductWithTenant3");
                    productWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }
               
                uow.Complete();
            }
        }
    }
}
