using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dapper.Filters.Query;
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
        private IDapperRepository<Product> ProductDapperRepository => Resolve<IDapperRepository<Product>>();
        private IRepository<Product> ProductRepository => Resolve<IRepository<Product>>();
        private IUnitOfWorkManager UnitOfWorkManager => Resolve<IUnitOfWorkManager>();
        private IRepository<ProductDetail> ProductDetailRepository => Resolve<IRepository<ProductDetail>>();
        private IDapperRepository<ProductDetail> ProductDetailDapperRepository => Resolve<IDapperRepository<ProductDetail>>();
        private IRepository<Person> PersonRepository => Resolve<IRepository<Person>>();
        private IDapperRepository<Person> PersonDapperRepository => Resolve<IDapperRepository<Person>>();
        private IDapperRepository<Good> GoodDapperRepository => Resolve<IDapperRepository<Good>>();
        private IDapperQueryFilterExecuter DapperQueryFilterExecuter => Resolve<IDapperQueryFilterExecuter>();

        [Fact]
        public async Task Dapper_Repository_Tests()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            using (IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin())
            {
                //---Insert operation should work and tenant, creation audit properties must be set---------------------
                await ProductDapperRepository.InsertAsync(new Product("TShirt"));
                Product insertedProduct = await ProductDapperRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");

                insertedProduct.ShouldNotBeNull();
                insertedProduct.TenantId.ShouldBe(AbpSession.TenantId);
                ((DateTime?) insertedProduct.CreationTime).ShouldNotBe(null);
                insertedProduct.CreatorUserId.ShouldBe(AbpSession.UserId);

                //----Update operation should work and Modification Audits should be set---------------------------
                await ProductDapperRepository.InsertAsync(new Product("TShirt"));
                Product productToUpdate = await ProductDapperRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");
                productToUpdate.Name = "Pants";
                await ProductDapperRepository.UpdateAsync(productToUpdate);

                productToUpdate.ShouldNotBeNull();
                productToUpdate.TenantId.ShouldBe(AbpSession.TenantId);
                ((DateTime?) productToUpdate.CreationTime).ShouldNotBe(null);
                productToUpdate.LastModifierUserId.ShouldBe(AbpSession.UserId);

                //---Get method should return single-------------------------------------------------------------------
                await ProductDapperRepository.InsertAsync(new Product("TShirt"));
                Action getAction = () => ProductDapperRepository.Single(x => x.Name == "TShirt");

                getAction.ShouldThrow<InvalidOperationException>("Sequence contains more than one element");

                //----Select * from syntax should work---------------------------------
                var queryResult = await ProductDapperRepository.QueryAsync("select * from Products");

                IEnumerable<Product> products = queryResult;

                products.Count().ShouldBeGreaterThan(0);

                //------------Ef and Dapper should work under same transaction---------------------
                Product productFromEf = await ProductRepository.FirstOrDefaultAsync(x => x.Name == "TShirt");
                Product productFromDapper = await ProductDapperRepository.SingleAsync(productFromEf.Id);

                productFromDapper.Name.ShouldBe(productFromEf.Name);
                productFromDapper.TenantId.ShouldBe(productFromEf.TenantId);

                //------Soft Delete should work for Dapper--------------
                await ProductDapperRepository.InsertAsync(new Product("SoftDeletableProduct"));

                Product toSoftDeleteProduct = await ProductDapperRepository
                    .SingleAsync(x => x.Name == "SoftDeletableProduct");

                await ProductDapperRepository.DeleteAsync(toSoftDeleteProduct);

                toSoftDeleteProduct.IsDeleted.ShouldBe(true);
                toSoftDeleteProduct.DeleterUserId.ShouldBe(AbpSession.UserId);
                toSoftDeleteProduct.TenantId.ShouldBe(AbpSession.TenantId);

                Product softDeletedProduct = await ProductRepository
                    .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                softDeletedProduct.ShouldBeNull();

                Product softDeletedProductFromDapper = await ProductDapperRepository
                    .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                softDeletedProductFromDapper.ShouldBeNull();

                using (UnitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    Product softDeletedProductWhenFilterDisabled = await ProductRepository
                        .FirstOrDefaultAsync(x => x.Name == "SoftDeletableProduct");

                    softDeletedProductWhenFilterDisabled.ShouldNotBeNull();

                    Product softDeletedProductFromDapperWhenFilterDisabled = await ProductDapperRepository
                        .SingleAsync(x => x.Name == "SoftDeletableProduct");

                    softDeletedProductFromDapperWhenFilterDisabled.ShouldNotBeNull();
                }

                using (AbpSession.Use(2, 266))
                {
                    int productWithTenant2Id = await ProductDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenant2"));

                    var productWithTenant2 = await ProductRepository.GetAsync(productWithTenant2Id);

                    productWithTenant2.TenantId
                        .ShouldBe(1); // Not sure about that?,Because we changed TenantId to 2 in this scope !!! Abp-TenantId = 2 now NOT 1 !!!
                }

                using (UnitOfWorkManager.Current.SetTenantId(3))
                {
                    int productWithTenant3Id = await ProductDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenant3"));

                    Product productWithTenant3 = await ProductRepository.GetAsync(productWithTenant3Id);

                    productWithTenant3.TenantId.ShouldBe(3);
                }

                Product productWithTenantId3FromDapper = await ProductDapperRepository
                    .FirstOrDefaultAsync(x => x.Name == "ProductWithTenant3");

                productWithTenantId3FromDapper.ShouldBeNull();

                Product p = await ProductDapperRepository.FirstOrDefaultAsync(x => x.IsDeleted == false);
                p.ShouldNotBeNull();

                using (UnitOfWorkManager.Current.SetTenantId(3))
                {
                    Product productWithTenantId3FromDapperInsideTenantScope = await ProductDapperRepository
                        .FirstOrDefaultAsync(x => x.Name == "ProductWithTenant3");

                    productWithTenantId3FromDapperInsideTenantScope.ShouldNotBeNull();
                }

                //About issue-#2091
                using (UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int productWithTenantId40 = await ProductDapperRepository
                        .InsertAndGetIdAsync(new Product("ProductWithTenantId40"));

                    Product productWithTenant40 = await ProductRepository.GetAsync(productWithTenantId40);

                    productWithTenant40.TenantId.ShouldBe(AbpSession.TenantId);
                    productWithTenant40.CreatorUserId.ShouldBe(AbpSession.UserId);
                }

                //Second DbContext tests
                var productDetailId = await ProductDetailRepository
                    .InsertAndGetIdAsync(new ProductDetail("Woman"));

                (await ProductDetailDapperRepository.GetAsync(productDetailId)).ShouldNotBeNull();

                await uow.CompleteAsync();
            }
        }

        //About issue-#3990
        [Fact]
        public void Should_Insert_Only_Have_IMustHaveTenant()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            using (IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin())
            {
                using (UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    int personWithTenantId40 =
                        PersonDapperRepository.InsertAndGetId(new Person("PersonWithTenantId40"));

                    Person personWithTenant40 = PersonRepository.Get(personWithTenantId40);

                    personWithTenant40.TenantId.ShouldBe(AbpSession.TenantId.Value);
                }
            }
        }

        [Fact]
        public async Task Dapper_Repository_Count_Should_Return_Correct_Value_For_Nullable_Int_Filter()
        {
            if (!OperatingSystem.IsWindows())
            {
                return;
            }

            using (IUnitOfWorkCompleteHandle uow = UnitOfWorkManager.Begin())
            {
                using (UnitOfWorkManager.Current.SetTenantId(AbpSession.TenantId))
                {
                    await GoodDapperRepository.InsertAsync(new Good {Name = "AbpTest"});
                    await UnitOfWorkManager.Current.SaveChangesAsync();

                    int? id = 1;

                    var dapperCount = await GoodDapperRepository.CountAsync(a => a.Id != id && a.Name == "AbpTest");
                    dapperCount.ShouldBe(0);
                }

                await uow.CompleteAsync();
            }
        }
    }
}
