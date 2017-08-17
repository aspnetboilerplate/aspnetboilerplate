using Abp.Domain.Repositories;
using Abp.EntityFramework.Repositories;
using Abp.Extensions;
using Abp.TestBase.SampleApplication.Crm;
using Abp.TestBase.SampleApplication.EntityFramework;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.EntityFramework
{
    public class EfRepositoryExtensions_Tests : SampleApplicationTestBase
    {
        private readonly IRepository<Company> _companyRepository;

        public EfRepositoryExtensions_Tests()
        {
            _companyRepository = Resolve<IRepository<Company>>();
        }

        [Fact]
        public void Should_Get_DbContext()
        {
            _companyRepository.GetDbContext().ShouldBeOfType<SampleApplicationDbContext>();
        }

        [Fact]
        public void Should_Get_IocResolver()
        {
            _companyRepository.As<AbpRepositoryBase<Company, int>>().IocResolver.ShouldNotBeNull();
        }
    }
}