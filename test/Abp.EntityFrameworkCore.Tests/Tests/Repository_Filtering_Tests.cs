using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Abp.EntityFrameworkCore.Tests.Tests
{
    public class Repository_Filtering_Tests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Post, Guid> _postRepository;
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<TicketListItem> _ticketListItemRepository;

        public Repository_Filtering_Tests()
        {
            _unitOfWorkManager = Resolve<IUnitOfWorkManager>();

            _postRepository = Resolve<IRepository<Post, Guid>>();
            _ticketRepository = Resolve<IRepository<Ticket>>();
            _ticketListItemRepository = Resolve<IRepository<TicketListItem>>();
        }

        override protected void PostInitialize()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
        }

        [Fact]
        public async Task Should_Filter_SoftDelete()
        {
            var posts = await _postRepository.GetAllListAsync();
            posts.All(p => !p.IsDeleted).ShouldBeTrue();
        }

        [Fact]
        public async Task Should_Get_SoftDeleted_Entities_If_Filter_Is_Disabled()
        {
            using (var uow = _unitOfWorkManager.Begin())
            {
                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
                {
                    var posts = await _postRepository.GetAllListAsync();
                    posts.Any(p => p.IsDeleted).ShouldBeTrue();
                }
            }
        }

        [Fact]
        public async Task Should_Filter_MayHaveTenantId()
        {
            var postsDefault = await _postRepository.GetAllListAsync();
            postsDefault.Any(p => p.TenantId == null).ShouldBeTrue();

            //Switch to tenant 42
            AbpSession.TenantId = 42;

            var posts1 = await _postRepository.GetAllListAsync();
            posts1.All(p => p.TenantId == 42).ShouldBeTrue();

            //Switch to host
            AbpSession.TenantId = null;
            
            var posts2 = await _postRepository.GetAllListAsync();
            posts2.Any(p => p.TenantId == 42).ShouldBeFalse();

            using (var uow = _unitOfWorkManager.Begin())
            {
                //Switch to tenant 42
                using (_unitOfWorkManager.Current.SetTenantId(42))
                {
                    var posts3 = await _postRepository.GetAllListAsync(p => p.Title != null);
                    posts3.All(p => p.TenantId == 42).ShouldBeTrue();
                }

                var posts4 = await _postRepository.GetAllListAsync();
                posts4.Any(p => p.TenantId == 42).ShouldBeFalse();
                posts4.Any(p => p.TenantId == null).ShouldBeTrue();

                using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var posts5 = await _postRepository.GetAllListAsync();
                    posts5.Any(p => p.TenantId == 42).ShouldBeTrue();
                    posts5.Any(p => p.TenantId == null).ShouldBeTrue();
                }
            }
        }

        [Fact]
        public async Task Should_Filter_MustHaveTenantId()
        {
            //Should get all entities for the host
            var ticketsDefault = await _ticketRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.TenantId == 1).ShouldBeTrue();
            ticketsDefault.Any(t => t.TenantId == 42).ShouldBeTrue();

            //Switch to tenant 42
            AbpSession.TenantId = 42;
            ticketsDefault = await _ticketRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.TenantId == 42).ShouldBeTrue();
            ticketsDefault.Any(t => t.TenantId != 42).ShouldBeFalse();

            //TODO: Create unit test
            //TODO: Change filter
        }

        [Fact]
        public async Task Should_Filter_View_With_MustHaveTenantId()
        {
            //Should get all entities for the host
            var ticketsDefault = await _ticketListItemRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.TenantId == 1).ShouldBeTrue();
            ticketsDefault.Any(t => t.TenantId == 42).ShouldBeTrue();

            //Switch to tenant 42
            AbpSession.TenantId = 42;
            ticketsDefault = await _ticketListItemRepository.GetAllListAsync();
            ticketsDefault.Any(t => t.TenantId == 42).ShouldBeTrue();
            ticketsDefault.Any(t => t.TenantId != 42).ShouldBeFalse();
        }
    }
}
