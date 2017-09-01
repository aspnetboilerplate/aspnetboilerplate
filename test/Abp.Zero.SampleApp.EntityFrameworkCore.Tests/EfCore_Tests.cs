using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.Zero.SampleApp.Roles;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.EntityFrameworkCore.Tests
{
    public class EfCore_Tests : SimpleTaskAppTestBase
    {
        private readonly RoleManager _roleManager;

        public EfCore_Tests()
        {
            _roleManager = Resolve<RoleManager>();
        }

        [Fact]
        public async Task Seed_Data_Test()
        {
            await UsingDbContextAsync(async context =>
            {
                (await context.Tenants.CountAsync()).ShouldBe(1);
            });
        }

        [Fact]
        public async Task Should_Create_And_Retrieve_Role()
        {
            await CreateRoleAsync("Role1");

            var role1Retrieved = await _roleManager.FindByNameAsync("Role1");
            role1Retrieved.ShouldNotBe(null);
            role1Retrieved.Name.ShouldBe("Role1");
        }

        [Fact]
        public async Task Multi_Tenancy_Tests_Using_Session()
        {
            //Switch to host
            AbpSession.TenantId = null;

            UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

            await CreateRoleAsync("HostRole1");

            UsingDbContext(context => { context.Roles.Count().ShouldBe(1); });

            //Switch to tenant 1
            AbpSession.TenantId = 1;

            UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

            await CreateRoleAsync("TenantRole1");

            UsingDbContext(context => { context.Roles.Count().ShouldBe(1); });
        }

        [Fact]
        public async Task Multi_Tenancy_Tests_Using_UOW()
        {
            var uowManager = Resolve<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                using (uowManager.Current.SetTenantId(null)) //Switch to host
                {
                    UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

                    await CreateRoleAsync("HostRole1");

                    UsingDbContext(context =>
                    {
                        context.Roles.Count().ShouldBe(1);
                        context.Roles.First().Name.ShouldBe("HostRole1");
                    });

                    using (uowManager.Current.SetTenantId(1)) //Switch to tenant 1
                    {
                        UsingDbContext(context => { context.Roles.Count().ShouldBe(0); });

                        await CreateRoleAsync("TenantRole1");

                        UsingDbContext(context =>
                        {
                            context.Roles.Count().ShouldBe(1);
                            context.Roles.First().Name.ShouldBe("TenantRole1");
                        });
                    }

                    //Automatically re-stored to host
                    UsingDbContext(context =>
                    {
                        context.Roles.Count().ShouldBe(1);
                        context.Roles.First().Name.ShouldBe("HostRole1");
                    });
                }

                await uow.CompleteAsync();
            }
        }

        protected async Task<Role> CreateRoleAsync(string name)
        {
            return await CreateRoleAsync(name, name);
        }

        protected async Task<Role> CreateRoleAsync(string name, string displayName)
        {
            var role = new Role(AbpSession.TenantId, name, displayName);

            (await _roleManager.CreateAsync(role)).Succeeded.ShouldBe(true);

            var uowManager = Resolve<IUnitOfWorkManager>();
            if (uowManager.Current != null)
            {
                await uowManager.Current.SaveChangesAsync();
            }

            await UsingDbContextAsync(async context =>
            {
                var createdRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == name);
                createdRole.ShouldNotBe(null);
            });

            return role;
        }
    }
}