using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Application.Services;
using AbpAspNetCoreDemo.Core.Application.Dtos;
using AbpAspNetCoreDemo.Core.Domain;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<List<ProductDto>> GetAllAsync()
        {
            return ObjectMapper.Map<List<ProductDto>>(await _productRepository.GetAllListAsync());
        }
        
        public int CreateProduct(ProductCreateInput input)
        {
            var product = ObjectMapper.Map<Product>(input);
            return _productRepository.InsertAndGetId(product);
        }

        public void CreateProductAndRollback(ProductCreateInput input)
        {
            _productRepository.Insert(ObjectMapper.Map<Product>(input));
            CurrentUnitOfWork.SaveChanges();
            throw new UserFriendlyException("This exception is thrown to rollback the transaction!");
        }

        //TODO: This method crashes!
        public async Task GetAllParallel()
        {
            const int threadCount = 32;

            var tasks = new List<Task<int>>();

            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(GetAllParallelMethod());
            }

            await Task.WhenAll(tasks.Cast<Task>().ToArray());

            foreach (var task in tasks)
            {
                Debug.Assert(task.Result > 0);
            }
        }

        [UnitOfWork(TransactionScopeOption.RequiresNew)]
        protected virtual async Task<int> GetAllParallelMethod()
        {
            return await _productRepository.CountAsync();
        }
    }
}
