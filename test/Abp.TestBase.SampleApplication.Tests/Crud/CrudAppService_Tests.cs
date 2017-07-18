using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.TestBase.SampleApplication.Crm;
using NSubstitute;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;

namespace Abp.TestBase.SampleApplication.Tests.Crud
{
    public class CrudAppService_Tests : SampleApplicationTestBase
    {
        private readonly CompanyAppService _companyAppService;
        private readonly AsyncCompanyAppService _asyncCompanyAppService;


        public CrudAppService_Tests()
        {
            _companyAppService = Resolve<CompanyAppService>();
            _asyncCompanyAppService = Resolve<AsyncCompanyAppService>();
        }

        [Fact]
        public void Should_Not_Get_All_Companies_If_Not_Authorized()
        {
            //Arrange
            _companyAppService.PermissionChecker = GetBlockerPermissionsChecker();

            //Act

            Should.Throw<AbpAuthorizationException>(() =>
            {
                _companyAppService.GetAll(new PagedAndSortedResultRequestDto());
            });
        }

        [Fact]
        public void Should_Get_All_Companies_If_Authorized()
        {
            _companyAppService.GetAll(new PagedAndSortedResultRequestDto()).TotalCount.ShouldBe(2);
        }

        [Fact]
        public void Should_Not_Delete_Company_If_Not_Authorized()
        {
            //Arrange
            _asyncCompanyAppService.PermissionChecker = GetBlockerPermissionsChecker();

            //Act

            Should.Throw<AbpAuthorizationException>(async () =>
            {
                await _asyncCompanyAppService.Delete(new EntityDto(1));
            });
        }

        [Fact]
        public async void Should_Delete_Company_If_Authorized()
        {
            //Act

            await _asyncCompanyAppService.Delete(new EntityDto(1));
            (await _asyncCompanyAppService.GetAll(new PagedAndSortedResultRequestDto())).TotalCount.ShouldBe(1);
        }

        private IPermissionChecker GetBlockerPermissionsChecker()
        {
            var blockerPermissionChecker = Substitute.For<IPermissionChecker>();
            blockerPermissionChecker.ReturnsForAll(false);
            return blockerPermissionChecker;
        }
    }
}
