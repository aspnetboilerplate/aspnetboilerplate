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
using Microsoft.Extensions.DependencyInjection;
using AbpAspNetCoreDemo;
using System;

namespace AbpAspNetCoreDemo.Core.Application
{
    public class ProductAppService : ApplicationService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IServiceProvider _serviceProvider;

        public ProductAppService(IRepository<Product> productRepository, IServiceProvider serviceProvider)
        {
            _productRepository = productRepository;
            _serviceProvider = serviceProvider;
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

        public async Task GetAllParallel()
        {
            const int threadCount = 32;

            var tasks = new List<Task<int>>();

            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Run(() => GetAllParallelMethod()));
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var productRepository = scope.ServiceProvider.GetRequiredService<IRepository<Product>>();
                return await productRepository.CountAsync();
            }
        }
    }
}